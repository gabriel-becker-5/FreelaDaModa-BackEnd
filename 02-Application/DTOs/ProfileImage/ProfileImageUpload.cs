namespace _02_Application.DTOs.ProfileImage
{
    public sealed class ProfileImageUpload
    {
        public Stream Content { get; set; }
        public string OriginalFileName { get; set; }
        public string? DeclaredContentType { get; set; }
        public long DeclaredLength { get; set; }

        public ProfileImageUpload(Stream content, string originalFileName, string? declaredContentType, long declaredLength)
        {
            Content = content;
            OriginalFileName = originalFileName;
            DeclaredContentType = declaredContentType;
            DeclaredLength = declaredLength;
        }
    }
}