using System.ComponentModel.DataAnnotations;

namespace _01_Presentation.Requests
{
    public sealed class UploadProfileImageRequest
    {
        [Required]
        public IFormFile Image { get; init; } = default!;
    }
}