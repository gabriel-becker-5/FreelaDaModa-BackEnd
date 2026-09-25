namespace _02_Application.Enums
{
    public enum ProfileImageUpdateResult
    {
        Success,
        UserNotFound,
        EmptyFile,
        FileTooLarge,
        UnsupportedFormat,
        Conflict,
        StorageError
    }
}