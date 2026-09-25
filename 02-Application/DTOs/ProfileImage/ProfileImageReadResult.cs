namespace _02_Application.DTOs.ProfileImage
{
    public class ProfileImageReadResult
    {
        public Stream Content { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string ETag { get; set; }
    }
}