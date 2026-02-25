using BlazorMerge.Files;
using Microsoft.Extensions.Logging;

namespace BlazorMerge.UnitTests.Files;

public sealed class FileManagerTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly FileManager _sut;
    private readonly StubLogger<FileManager> _logger;

    public FileManagerTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"FileManagerTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
        _logger = new StubLogger<FileManager>();
        _sut = new FileManager(_logger);
    }

    [Fact]
    public void ReadFile_WhenFileExists_ReturnsContent()
    {
        // arrange
        var filePath = Path.Combine(_testDirectory, "test.txt");
        const string content = "Hello, World!";
        File.WriteAllText(filePath, content);

        // act
        var result = _sut.ReadFile(filePath);

        // assert
        result.Should()
            .Be(content);
    }

    [Fact]
    public void ReadFile_WhenFileDoesNotExist_ThrowsFileNotFoundException()
    {
        // arrange
        var filePath = Path.Combine(_testDirectory, "nonexistent.txt");

        // act
        var act = () => _sut.ReadFile(filePath);

        // assert
        act.Should()
            .Throw<FileNotFoundException>()
            .WithMessage($"File {filePath} does not exist");
    }

    [Fact]
    public void WriteFile_WhenCalled_CreatesFileWithContent()
    {
        // arrange
        var filePath = Path.Combine(_testDirectory, "output.txt");
        const string content = "Test content";

        // act
        _sut.WriteFile(filePath, content);

        // assert
        File.Exists(filePath)
            .Should()
            .BeTrue();
        var fileContent = File.ReadAllText(filePath);
        fileContent.Should()
            .Be(content);
    }

    [Fact]
    public void DeleteFile_WhenFileExists_DeletesFile()
    {
        // arrange
        var filePath = Path.Combine(_testDirectory, "to-delete.txt");
        File.WriteAllText(filePath, "To be deleted");

        // act
        _sut.DeleteFile(filePath);

        // assert
        File.Exists(filePath)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void DeleteFile_WhenFileDoesNotExist_ThenLogThatFileIsMissing()
    {
        // arrange
        var filePath = Path.Combine(_testDirectory, "nonexistent.txt");

        // act
        _sut.DeleteFile(filePath);

        // assert
        _logger.AssertLogEvent(
            e =>
                e.LogLevel == LogLevel.Warning,
            ex =>
                ex.Message.Should()
                    .Contain("File does not exist, cannot delete:")
                    .And.Contain(filePath)
        );

        _logger.AssertLogCount(e => e.Message.Contains("Deleting file"), 0);
    }

    [Fact]
    public void ListSettingsFiles_WhenCalled_ReturnsMatchingFiles()
    {
        // arrange
        var file1 = Path.Combine(_testDirectory, "appsettings.json");
        var file2 = Path.Combine(_testDirectory, "appsettings.Development.json");
        var file3 = Path.Combine(_testDirectory, "other-file.txt");
        File.WriteAllText(file1, "{}");
        File.WriteAllText(file2, "{}");
        File.WriteAllText(file3, "Not a settings file");

        // act
        var result = _sut.ListSettingsFiles(_testDirectory, path => Path.GetFileName(path)
            .StartsWith("appsettings"));

        // assert
        result.Should()
            .Contain(["appsettings.json", "appsettings.Development.json"])
            .And.NotContain("other-file.txt");
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }
}