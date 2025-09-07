using System.Text.RegularExpressions;
using BlazorMerge.Files;

namespace BlazorMerge.Feature.Merge;

public partial class MergeService
{
    private readonly IFileManager _fileManager;
    private readonly IMerger _merger;

    public MergeService(IFileManager fileManager, IMerger merger)
    {
        _fileManager = fileManager;
        _merger = merger;
    }

    public int MergeEnvironment(MergeOptions options)
    {
        var mainFileName = Constants.MainFileName;
        var mainFilePath = ConstructPath(options.Path, mainFileName);
        var readAppSetting = _fileManager.ReadFile(mainFilePath);
        var environmentFileName = ReplacePath(options);
        var readEnvironmentSetting = _fileManager.ReadFile(ConstructPath(options.Path, environmentFileName));
        WriteNewSettingsFile(readAppSetting, readEnvironmentSetting, mainFilePath);
        DeleteSettingsFiles(options);
        WriteNewSettingsFile("{}", "{}", ConstructPath(options.Path, "appsettings.Production.json"));

        return 0;
    }

    private static string ConstructPath(string path, string fileName)
    {
        var computedPath = path.Trim().EndsWith('\\') || path.Trim().EndsWith('/') ? path : $"{path}\\";
        return $"{computedPath}{fileName}";
    }

    private void DeleteSettingsFiles(MergeOptions options)
    {
        var files = _fileManager.ListSettingsFiles(options.Path, OnlySettingsFiles);
        var settingsFiles = ParseSettingsFiles(files);
        var jsonFilesToDelete = settingsFiles
            .Where(x => x is {Type: SettingsFileType.Environment, Extension: SettingsFileExtension.Json}).ToList();
        var brotliFilesToDelete = settingsFiles.Where(x => x is {Extension: SettingsFileExtension.Brotli}).ToList();
        var gzipFilesToDelete = settingsFiles.Where(x => x is {Extension: SettingsFileExtension.Gzip}).ToList();

        DeleteFiles(jsonFilesToDelete, options);
        DeleteFiles(brotliFilesToDelete, options);
        DeleteFiles(gzipFilesToDelete, options);
    }

    private static bool OnlySettingsFiles(string fileName)
    {
        var match = IsAppSettingsFile().Match(fileName);
        return match.Success;
    }

    private static IList<SettingsFile> ParseSettingsFiles(IEnumerable<string> files)
    {
        var settingsFiles = new List<SettingsFile>();
        foreach (var file in files)
        {
            var settingsFile = new SettingsFile();
            var split = file.Split('.');
            settingsFile.FileName = file;
            settingsFile.Type = split[1] switch
            {
                "Dev" => SettingsFileType.Environment,
                "Development" => SettingsFileType.Environment,
                "Production" => SettingsFileType.Environment,
                "Staging" => SettingsFileType.Environment,
                _ => SettingsFileType.Primary
            };
            settingsFile.Extension = split[^1] switch
            {
                "json" => SettingsFileExtension.Json,
                "br" => SettingsFileExtension.Brotli,
                "gz" => SettingsFileExtension.Gzip,
                _ => throw new ArgumentOutOfRangeException(file, "File extension not supported")
            };
            settingsFiles.Add(settingsFile);
        }

        return settingsFiles;
    }

    private void DeleteFiles(IEnumerable<SettingsFile> files, MergeOptions options)
    {
        foreach (var file in files.Select(file => ConstructPath(options.Path, file.FileName)))
        {
            _fileManager.DeleteFile(file);
        }
    }

    private void WriteNewSettingsFile(string readAppSetting, string readEnvironmentSetting, string mainFileName)
    {
        var merged = _merger.Merge(readAppSetting, readEnvironmentSetting);
        _fileManager.WriteFile(mainFileName, merged);
    }

    private static string ReplacePath(MergeOptions options)
    {
        var environment = options.Environment;
        var environmentFileNameFormat = Constants.EnvironmentFileName;
        
        return environmentFileNameFormat.Replace("{environment}", environment);
    }

    [GeneratedRegex(@"appsettings(\.\w+)?\.(json|br|gz)$")]
    private static partial Regex IsAppSettingsFile();
}