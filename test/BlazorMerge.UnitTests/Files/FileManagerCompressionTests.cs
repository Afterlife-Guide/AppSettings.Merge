using System.IO.Compression;
using System.Text;
using BlazorMerge.Files;

namespace BlazorMerge.UnitTests.Files;

public sealed class FileManagerCompressionTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly FileManager _sut;

    public FileManagerCompressionTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"FileManagerTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
        var logger = new StubLogger<FileManager>();
        _sut = new FileManager(logger);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }

    [Fact]
    public void When_WritingGzipFile_Then_FileShouldBeCompressed()
    {
        // arrange
        const string content = "{\"key\": \"value\", \"nested\": {\"property\": \"test\"}}";
        var gzipPath = Path.Combine(_testDirectory, "appsettings.json.gz");

        // act
        _sut.WriteGzipFile(gzipPath, content);

        // assert
        File.Exists(gzipPath).Should().BeTrue();
        
        using var fileStream = File.OpenRead(gzipPath);
        using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
        using var reader = new StreamReader(gzipStream, Encoding.UTF8);
        var decompressed = reader.ReadToEnd();
        
        decompressed.Should().Be(content);
    }

    [Fact]
    public void When_WritingBrotliFile_Then_FileShouldBeCompressed()
    {
        // arrange
        const string content = "{\"key\": \"value\", \"nested\": {\"property\": \"test\"}}";
        var brotliPath = Path.Combine(_testDirectory, "appsettings.json.br");

        // act
        _sut.WriteBrotliFile(brotliPath, content);

        // assert
        File.Exists(brotliPath).Should().BeTrue();
        
        using var fileStream = File.OpenRead(brotliPath);
        using var brotliStream = new BrotliStream(fileStream, CompressionMode.Decompress);
        using var reader = new StreamReader(brotliStream, Encoding.UTF8);
        var decompressed = reader.ReadToEnd();
        
        decompressed.Should().Be(content);
    }
}
