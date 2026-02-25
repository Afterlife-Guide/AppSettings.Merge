using System.Text.RegularExpressions;
using BlazorMerge.Files;

namespace BlazorMerge.Feature.Merge;

public partial class MergeService(IFileManager fileManager, IMerger merger)
{
    public int MergeEnvironment(MergeOptions options)
    {
        const string mainFileName = Constants.MainFileName;
        var mainFilePath = ConstructPath(options.Path, mainFileName);
        var readAppSetting = fileManager.ReadFile(mainFilePath);
        var environmentFileName = ReplacePath(options);
        var readEnvironmentSetting = fileManager.ReadFile(ConstructPath(options.Path, environmentFileName));
        WriteNewSettingsFile(readAppSetting, readEnvironmentSetting, mainFilePath);
        DeleteSettingsFiles(options);
        WriteNewSettingsFile("{}", "{}", ConstructPath(options.Path, "appsettings.Production.json"));

        return 0;
    }

    private static string ConstructPath(string path, string fileName)
    {
        var computedPath = path.Trim()
            .EndsWith('\\') || path.Trim()
            .EndsWith('/')
            ? path
            : $"{path}\\";
        return $"{computedPath}{fileName}";
    }

    private void DeleteSettingsFiles(MergeOptions options)
    {
        var files = fileManager.ListSettingsFiles(options.Path, OnlySettingsFiles);
        var settingsFiles = ParseSettingsFiles(files);
        var jsonFilesToDelete = settingsFiles
            .Where(x => x is {Type: SettingsFileType.Environment, Extension: SettingsFileExtension.Json})
            .ToList();
        var brotliFilesToDelete = settingsFiles.Where(x => x is
                {Extension: SettingsFileExtension.Brotli})
            .ToList();
        var gzipFilesToDelete = settingsFiles.Where(x => x is
                {Extension: SettingsFileExtension.Gzip})
            .ToList();

        DeleteFiles(jsonFilesToDelete, options);
        DeleteFiles(brotliFilesToDelete, options);
        DeleteFiles(gzipFilesToDelete, options);
    }

    private static bool OnlySettingsFiles(string fileName)
    {
        var match = IsAppSettingsFile()
            .Match(fileName);
        return match.Success;
    }

    private static List<SettingsFile> ParseSettingsFiles(IEnumerable<string> files)
    {
        var settingsFiles = new List<SettingsFile>();
        foreach (var file in files)
        {
            var settingsFile = new SettingsFile();
            var split = file.Split('.');
            settingsFile.FileName = file;
            settingsFile.Type = split[1] switch
            {
                "Dev" or "Development" or "Production" or "Staging" => SettingsFileType.Environment,
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
            fileManager.DeleteFile(file);
        }
    }

    private void WriteNewSettingsFile(string readAppSetting, string readEnvironmentSetting, string mainFileName)
    {
        var merged = merger.Merge(readAppSetting, readEnvironmentSetting);
        fileManager.WriteFile(mainFileName, merged);
        fileManager.WriteGzipFile($"{mainFileName}.gz", merged);
        fileManager.WriteBrotliFile($"{mainFileName}.br", merged);
    }

    private static string ReplacePath(MergeOptions options)
    {
        var environment = options.Environment;
        const string environmentFileNameFormat = Constants.EnvironmentFileName;

        return environmentFileNameFormat.Replace("{environment}", environment);
    }

    [GeneratedRegex(@"appsettings(\.\w+)?\.((json(\.(br|gz))?)|br|gz)$")]
    private static partial Regex IsAppSettingsFile();
}