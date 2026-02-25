using BlazorMerge.Feature.Merge;
using BlazorMerge.Files;
using CommandLine;

namespace BlazorMerge.UnitTests.Merge;

public class MergeServiceTests
{
    private readonly MergeService _sut;
    private readonly IFileManager _mockFileManager = Substitute.For<IFileManager>();
    private readonly IMerger _mockMerger = Substitute.For<IMerger>();
    private const string DefaultPath = @"Z:\my-project\wwwroot\";

    public MergeServiceTests()
    {
        _sut = new MergeService(_mockFileManager, _mockMerger);
    }

    [Theory]
    [MemberData(nameof(Environments))]
    public void MergeEnvironment_WhenCalled_ThenShouldReadMainAppSettingFile(string environment)
    {
        // arrange
        var options = CreateDefaultOptions(environment);

        // act
        _sut.MergeEnvironment(options);

        // assert
        _mockFileManager
            .Received(1)
            .ReadFile($"{DefaultPath}{Constants.MainFileName}");
    }

    [Theory]
    [MemberData(nameof(Environments))]
    public void MergeEnvironment_WhenCalled_ThenShouldReadEnvironmentAppSettingFile(string environment)
    {
        // arrange
        var options = CreateDefaultOptions(environment);

        // act
        _sut.MergeEnvironment(options);

        // assert
        _mockFileManager
            .Received(1)
            .ReadFile($"{DefaultPath}appsettings.{environment}.json");
    }

    [Theory]
    [MemberData(nameof(Environments))]
    public void MergeEnvironment_WhenCalled_ThenShouldWriteMergedAppSettingFile(string environment)
    {
        // arrange
        var options = CreateDefaultOptions(environment);
        var appSettingContent = GetDefaultAppSettingContent();
        var environmentSettingContent = GetDefaultEnvironmentContent();
        const string appSettingPath = $"{DefaultPath}appsettings.json";
        _mockFileManager.ReadFile(appSettingPath)
            .Returns(appSettingContent);
        _mockFileManager.ReadFile($"{DefaultPath}appsettings.{environment}.json")
            .Returns(environmentSettingContent);
        const string expectedMergedContent = "MergedContent";
        _mockMerger.Merge(appSettingContent, environmentSettingContent)
            .Returns(expectedMergedContent);


        // act
        _sut.MergeEnvironment(options);

        // assert
        var settingsJson = ExtractFileManagerArgument("WriteFile", appSettingPath);
        var settingsBrotli = ExtractFileManagerArgument("WriteBrotliFile", $"{appSettingPath}.br");
        var settingsGzip = ExtractFileManagerArgument("WriteGzipFile", $"{appSettingPath}.gz");

        settingsJson.Should()
            .Be(expectedMergedContent);
        settingsBrotli.Should()
            .Be(expectedMergedContent);
        settingsGzip.Should()
            .Be(expectedMergedContent);
    }

    [Theory]
    [MemberData(nameof(Environments))]
    public void MergeEnvironment_WhenCalled_ThenShouldDeleteCorrectAppSettingFiles(string environment)
    {
        // arrange
        var options = CreateDefaultOptions(environment);
        var settingsFiles = new List<string>
        {
            "appsettings.json",
            "appsettings.json.br",
            "appsettings.json.gz",
            "appsettings.Dev.json",
            "appsettings.Dev.br",
            "appsettings.Dev.gz",
            "appsettings.Staging.json",
            "appsettings.Staging.br",
            "appsettings.Staging.gz",
            "appsettings.Production.json",
            "appsettings.Production.br",
            "appsettings.Production.gz"
        };
        _mockFileManager.ListSettingsFiles(Arg.Any<string>(), Arg.Any<Func<string, bool>>())
            .Returns(settingsFiles);

        // act
        _sut.MergeEnvironment(options);

        // assert
        foreach (var file in settingsFiles.Where(file => file != "appsettings.json"))
        {
            _mockFileManager.Received(1)
                .DeleteFile($"{DefaultPath}{file}");
        }
    }

    [Theory]
    [MemberData(nameof(Environments))]
    public void MergeEnvironment_WhenCalled_ThenShouldEmptyProductionAppSettingFile(string environment)
    {
        // arrange
        var options = CreateDefaultOptions(environment);
        _mockMerger.Merge("{}", "{}")
            .Returns("{}");

        // act
        _sut.MergeEnvironment(options);

        // assert
        _mockFileManager.Received(1)
            .WriteFile($"{DefaultPath}appsettings.Production.json", "{}");
        _mockFileManager.Received(1)
            .WriteBrotliFile($"{DefaultPath}appsettings.Production.json.br", "{}");
        _mockFileManager.Received(1)
            .WriteGzipFile($"{DefaultPath}appsettings.Production.json.gz", "{}");
    }

    private string ExtractFileManagerArgument(string methodName, string filePath)
    {
        var calls = _mockFileManager
            .ReceivedCalls()
            .Where(c => c.GetMethodInfo()
                .Name.Equals(methodName))
            .Select(x => x.GetArguments())
            .Last(x => x[0] is not null && x[0]!.Equals(filePath));

        return calls[^1]
            .Cast<string>();
    }

    private static string GetDefaultAppSettingContent() =>
        """
        {
            "Default": {
                "Key": "Value"
            }
        }
        """;

    private static string GetDefaultEnvironmentContent() =>
        """
        {
            "Default": {
                "Key": "Other"
            }
        }
        """;

    public static TheoryData<string> Environments =>
    [
        "Development",
        "Staging",
        "Production"
    ];

    private static MergeOptions CreateDefaultOptions(string environment)
    {
        return new MergeOptions
        {
            Environment = environment,
            Path = DefaultPath
        };
    }
}