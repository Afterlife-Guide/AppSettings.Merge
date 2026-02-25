using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BlazorMerge.Files;

public class FileManager : IFileManager
{
    private readonly ILogger<FileManager> _logger;

    public FileManager(ILogger<FileManager> logger)
    {
        _logger = logger;
    }

    public string ReadFile(string path)
    {
        if (!FileDoesNotExist(path)) return File.ReadAllText(path);
        _logger.LogInformation("File {Path} does not exist", path);
        throw new FileNotFoundException($"File {path} does not exist");

    }

    private static bool FileDoesNotExist(string path)
    {
        return !File.Exists(path);
    }

    public void WriteFile(string path, string content)
    {
        _logger.LogInformation("Writing file {Path}", path);
        File.WriteAllText(path, content);
    }

    public void DeleteFile(string path)
    {
        _logger.LogInformation("Deleting file {Path}", path);
        File.Delete(path);
    }

    public IList<string> ListSettingsFiles(string path, Func<string, bool> parser)
    {
        var files = Directory.GetFiles(path);

        return (from file in files
            where parser(file)
            select Path.GetFileName(file)).ToList();
    }

    public void WriteGzipFile(string path, string content)
    {
        _logger.LogInformation("Writing gzip file {Path}", path);
        var bytes = Encoding.UTF8.GetBytes(content);
        using var fileStream = File.Create(path);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress);
        gzipStream.Write(bytes, 0, bytes.Length);
    }

    public void WriteBrotliFile(string path, string content)
    {
        _logger.LogInformation("Writing brotli file {Path}", path);
        var bytes = Encoding.UTF8.GetBytes(content);
        using var fileStream = File.Create(path);
        using var brotliStream = new BrotliStream(fileStream, CompressionMode.Compress);
        brotliStream.Write(bytes, 0, bytes.Length);
    }
}