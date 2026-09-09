var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");
var versionNumber = Argument("versionNumber", "0.1.0");
var solutionFolder = "./";
var appProject = "./src/BlazorMerge/BlazorMerge.csproj";

Task("Clean")
    .Does(() =>
    {
        // Clean solution
        DotNetClean(solutionFolder);
    });

Task("Restore")
	.Does(() =>
	{
		// Restore NuGet packages
		DotNetRestore(solutionFolder);
	});

Task("Build")
	.Does(() =>
	{
		// Build solution
		DotNetBuild(solutionFolder, new DotNetBuildSettings
		{
			NoRestore = true,
			Configuration = configuration,
            ArgumentCustomization = args => args.Append("/p:Version=" + versionNumber)
		});
	});

Task("Test")
	.Does(() =>
	{
		// Run tests
		DotNetTest(solutionFolder, new DotNetTestSettings
		{
			NoRestore = true,
			Configuration = configuration,
            EnvironmentVariables = new Dictionary<string, string>
                                                    {
                                                        ["TESTINGPLATFORM_EXITCODE_IGNORE"] = "8"
                                                    }
		});
		
		var logFileName = $"results.xml";
        var testProjects = GetFiles("./test/**/*.*Tests.csproj");
        foreach (var project in testProjects)
        {
            var projectName = project.GetFilenameWithoutExtension().ToString();
            var dll = GetFiles($"{project.GetDirectory()}/bin/{configuration}/**/{projectName}.dll").First();
            var reportPath = project.GetDirectory().Combine("TestResults").CombineWithFilePath(logFileName);
            var testArguments = $"-result-junit \"{reportPath}\"";

            DotNetExecute(dll.FullPath, testArguments);
        }
	});

RunTarget(target);