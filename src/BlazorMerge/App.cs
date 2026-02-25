using BlazorMerge.Feature.Merge;
using CommandLine;
using Microsoft.Extensions.Configuration;

namespace BlazorMerge;

public class App
{
    private readonly MergeService _mergeServiceLegacy;

    public App(IConfiguration configuration, MergeService mergeServiceLegacy)
    {
        _mergeServiceLegacy = mergeServiceLegacy;
    }

    public void Run(IEnumerable<string> args)
    {
        Parser.Default.ParseArguments<MergeOptions>(args)
            .MapResult(
                (MergeOptions opts) => _mergeServiceLegacy.MergeEnvironment(opts),
                errs => 1
                );
    }
}