using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BlazorMerge.Files;

public partial class FileManager(ILogger<FileManager> logger) : IFileManager
{
    public string ReadFile(string path)
    {
        if (!FileDoesNotExist(path)) return File.ReadAllText(path);
        LogFilePathDoesNotExist(logger, path);
        throw new FileNotFoundException($"File {path} does not exist");
    }

    private static bool FileDoesNotExist(string path)
    {
        return !File.Exists(path);
    }

    public void WriteFile(string path, string content)
    {
        LogWritingFilePath(logger, path);
        File.WriteAllText(path, content);
    }

    public void DeleteFile(string path)
    {
        if (FileDoesNotExist(path))
        {
            logger.LogWarning("File does not exist, cannot delete: {Path}", path);
            return;
        }
        LogDeletingFilePath(logger, path);
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
        LogWritingGzipFilePath(logger, path);
        var bytes = Encoding.UTF8.GetBytes(content);
        using var fileStream = File.Create(path);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress);
        gzipStream.Write(bytes, 0, bytes.Length);
    }

    public void WriteBrotliFile(string path, string content)
    {
        LogWritingBrotliFilePath(logger, path);
        var bytes = Encoding.UTF8.GetBytes(content);
        using var fileStream = File.Create(path);
        using var brotliStream = new BrotliStream(fileStream, CompressionMode.Compress);
        brotliStream.Write(bytes, 0, bytes.Length);
    }

    [LoggerMessage(LogLevel.Information, "File {path} does not exist")]
    static partial void LogFilePathDoesNotExist(ILogger<FileManager> logger, string path);

    [LoggerMessage(LogLevel.Information, "Writing file {path}")]
    static partial void LogWritingFilePath(ILogger<FileManager> logger, string path);

    [LoggerMessage(LogLevel.Information, "Deleting file {path}")]
    static partial void LogDeletingFilePath(ILogger<FileManager> logger, string path);

    [LoggerMessage(LogLevel.Information, "Writing gzip file {path}")]
    static partial void LogWritingGzipFilePath(ILogger<FileManager> logger, string path);

    [LoggerMessage(LogLevel.Information, "Writing brotli file {path}")]
    static partial void LogWritingBrotliFilePath(ILogger<FileManager> logger, string path);
}