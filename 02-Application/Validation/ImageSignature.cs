namespace _02_Application.Validation
{
    public static class ImageSignature
    {
        public static bool IsJpeg(ReadOnlySpan<byte> bytes)
        {
            return bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF;
        }

        public static bool IsPng(ReadOnlySpan<byte> bytes)
        {
            return bytes.Length >= 8
                && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47
                && bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A;
        }
    }
}