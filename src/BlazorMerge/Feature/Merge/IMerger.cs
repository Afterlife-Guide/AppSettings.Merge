namespace BlazorMerge.Feature.Merge;

public interface IMerger
{
    public string Merge(string appSettingContent, string environmentSettingContent);
}