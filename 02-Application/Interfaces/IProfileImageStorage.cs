namespace _02_Application.Interfaces
{
    public interface IProfileImageStorage
    {
        Task SaveAsync(string fileName, byte[] content, CancellationToken cancellationToken = default);
        Stream? OpenRead(string fileName);
        Task DeleteAsync(string fileName);
    }
}