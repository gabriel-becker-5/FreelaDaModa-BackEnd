using _02_Application.Enums;

namespace _02_Application.DTOs
{
    public class CreateUserResult
    {
        public CreateUserStatus Status { get; set; }
        public int? UserId { get; set; }
        public ICollection<EnumValidationError> Errors { get; set; } = [];
    }
}
