using _02_Application.DTOs.ProfileImage;
using _02_Application.Enums;
using _02_Application.Interfaces;
using _02_Application.Validation;
using _04_Domain.Entities.Identity;
using _04_Domain.Interfaces;

namespace _02_Application.Services
{
    public class ProfileImageService : IProfileImageService
    {
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png"];

        private readonly IUserRepository _userRepository;
        private readonly IProfileImageStorage _storage;

        public ProfileImageService(IUserRepository userRepository, IProfileImageStorage storage)
        {
            _userRepository = userRepository;
            _storage = storage;
        }

        public async Task<ProfileImageUpdateResult> ReplaceAsync(int userId, ProfileImageUpload upload, CancellationToken cancellationToken = default)
        {
            byte[] content;
            try
            {
                content = await ReadLimitedAsync(upload.Content, MaxFileSizeBytes + 1, cancellationToken);
            }
            catch (EndOfStreamException) { return ProfileImageUpdateResult.StorageError; }

            if (content.Length == 0)
            {
                return ProfileImageUpdateResult.EmptyFile;
            }

            if (content.Length > MaxFileSizeBytes)
            {
                return ProfileImageUpdateResult.FileTooLarge;
            }

            string? extension = GetSafeExtension(upload.OriginalFileName);
            if (extension == null || !AllowedExtensions.Contains(extension))
            {
                return ProfileImageUpdateResult.UnsupportedFormat;
            }

            if (!ContentMatchesExtension(content, extension))
            {
                return ProfileImageUpdateResult.UnsupportedFormat;
            }

            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return ProfileImageUpdateResult.UserNotFound;
            }

            string? oldKey = user.ProfileImageKey;
            string newKey = Guid.NewGuid().ToString("N") + extension;

            try
            {
                await _storage.SaveAsync(newKey, content, cancellationToken);
            }
            catch
            {
                return ProfileImageUpdateResult.StorageError;
            }

            bool updated = await _userRepository.TrySetProfileImageKeyAsync(userId, oldKey, newKey);
            if (!updated)
            {
                await _storage.DeleteAsync(newKey); // compensação: arquivo órfão
                User? stillExists = await _userRepository.GetUserByIdAsync(userId);
                return stillExists == null ? ProfileImageUpdateResult.UserNotFound : ProfileImageUpdateResult.Conflict;
            }

            if (!string.IsNullOrEmpty(oldKey))
            {
                await _storage.DeleteAsync(oldKey); // melhor esforço; falha não desfaz o upload
            }

            return ProfileImageUpdateResult.Success;
        }

        public async Task<ProfileImageUpdateResult> RemoveAsync(int userId, CancellationToken cancellationToken = default)
        {
            User? user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return ProfileImageUpdateResult.UserNotFound;
            }

            string? currentKey = user.ProfileImageKey;
            if (string.IsNullOrEmpty(currentKey))
            {
                return ProfileImageUpdateResult.Success; // idempotente
            }

            bool updated = await _userRepository.TrySetProfileImageKeyAsync(userId, currentKey, null);
            if (!updated)
            {
                return ProfileImageUpdateResult.Conflict;
            }

            await _storage.DeleteAsync(currentKey); // melhor esforço
            return ProfileImageUpdateResult.Success;
        }

        public async Task<ProfileImageReadResult?> GetAsync(int userId, CancellationToken cancellationToken = default)
        {
            string? key = await _userRepository.GetProfileImageKeyAsync(userId);
            if (string.IsNullOrEmpty(key))
            {
                return null; // usuário inexistente/deletado ou sem foto
            }

            Stream? stream = _storage.OpenRead(key);
            if (stream == null)
            {
                return null; // referência órfã — registrar log para diagnóstico
            }

            return new ProfileImageReadResult
            {
                Content = stream,
                ContentType = key.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" : "image/jpeg",
                FileName = key,
                ETag = key
            };
        }

        private static async Task<byte[]> ReadLimitedAsync(Stream input, long maxBytes, CancellationToken ct)
        {
            using var buffer = new MemoryStream();
            var chunk = new byte[81920];
            int read;
            while ((read = await input.ReadAsync(chunk, ct)) > 0)
            {
                if (buffer.Length + read > maxBytes)
                {
                    throw new InvalidOperationException(); // excede o teto
                }
                buffer.Write(chunk, 0, read);
            }
            return buffer.ToArray();
        }

        private static string? GetSafeExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;
            if (fileName.IndexOfAny(['/', '\\', '\0']) >= 0) return null; // sem sintaxe de caminho
            string? extension = Path.GetExtension(fileName);
            return string.IsNullOrEmpty(extension) ? null : extension.ToLowerInvariant();
        }

        private static bool ContentMatchesExtension(byte[] content, string extension)
        {
            return extension switch
            {
                ".png" => ImageSignature.IsPng(content),
                ".jpg" or ".jpeg" => ImageSignature.IsJpeg(content),
                _ => false
            };
        }
    }
}