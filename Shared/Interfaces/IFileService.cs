namespace Shared.Services
{
    public interface IFileService
    {
        string LoadFromFile();
        bool SaveToFile(string content);
    }
}