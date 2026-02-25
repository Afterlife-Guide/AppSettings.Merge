using Newtonsoft.Json.Linq;

namespace BlazorMerge.Feature.Merge;

public class Merger : IMerger
{
    public string Merge(string appSettingContent, string environmentSettingContent)
    {
        var appSettingJObject = JObject.Parse(appSettingContent);
        var environmentSettingJObject = JObject.Parse(environmentSettingContent);
        appSettingJObject.Merge(environmentSettingJObject, new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Union,
            
        });
        return appSettingJObject.ToString();
    }
}