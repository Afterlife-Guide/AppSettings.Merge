using BlazorMerge;
using BlazorMerge.Feature.Merge;
using BlazorMerge.Files;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = CreateHostBuilder().Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;
try
{
    services.GetRequiredService<App>().Run(args);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

return;

IHostBuilder CreateHostBuilder()
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, s) =>
        {
            s.AddSingleton<App>();
            s.AddSingleton<MergeService>();
            s.AddSingleton<IFileManager, FileManager>();
            s.AddSingleton<IMerger, Merger>();
        });
}