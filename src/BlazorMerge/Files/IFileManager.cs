namespace BlazorMerge.Files;

public interface IFileManager
{
    string ReadFile(string path);
    void WriteFile(string path, string content);
    void DeleteFile(string path);
    IList<string> ListSettingsFiles(string path, Func<string, bool> parser);
    void WriteGzipFile(string path, string content);
    void WriteBrotliFile(string path, string content);
}