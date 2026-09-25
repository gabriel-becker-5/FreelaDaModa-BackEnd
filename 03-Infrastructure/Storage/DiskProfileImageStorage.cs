using _02_Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace _03_Infrastructure.Storage
{
    public partial class DiskProfileImageStorage : IProfileImageStorage
    {
        private readonly string _rootPath;

        public DiskProfileImageStorage(IOptions<DiskProfileImageStorageOptions> options)
        {
            _rootPath = Path.GetFullPath(options.Value.RootPath);
            Directory.CreateDirectory(_rootPath);
        }

        public async Task SaveAsync(string fileName, byte[] content, CancellationToken cancellationToken = default)
        {
            string fullPath = ResolveSafePath(fileName);
            await File.WriteAllBytesAsync(fullPath, content, cancellationToken);
        }

        public Stream? OpenRead(string fileName)
        {
            try
            {
                return File.OpenRead(ResolveSafePath(fileName));
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public Task DeleteAsync(string fileName)
        {
            try
            {
                File.Delete(ResolveSafePath(fileName));
            }
            catch (IOException) { } // melhor esforço
            catch (UnauthorizedAccessException) { }

            return Task.CompletedTask;
        }

        private string ResolveSafePath(string fileName)
        {
            if (!KeyPattern().IsMatch(fileName))
            {
                throw new ArgumentException("Nome de arquivo inválido.", nameof(fileName));
            }

            string fullPath = Path.GetFullPath(Path.Combine(_rootPath, fileName));
            if (!fullPath.StartsWith(_rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Caminho fora da raiz de armazenamento.", nameof(fileName));
            }

            return fullPath;
        }

        [GeneratedRegex("^[0-9a-f]{32}\\.(png|jpg|jpeg)$")]
        private static partial Regex KeyPattern();
    }
}