using _02_Application.DTOs.ProfileImage;
using _02_Application.Enums;

namespace _02_Application.Interfaces
{
    public interface IProfileImageService
    {
        Task<ProfileImageUpdateResult> ReplaceAsync(int userId, ProfileImageUpload upload, CancellationToken cancellationToken = default);
        Task<ProfileImageUpdateResult> RemoveAsync(int userId, CancellationToken cancellationToken = default);
        Task<ProfileImageReadResult?> GetAsync(int userId, CancellationToken cancellationToken = default);
    }
}