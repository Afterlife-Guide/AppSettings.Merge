using System.Runtime.CompilerServices;
using BlazorMerge.Feature.Merge;

namespace BlazorMerge.UnitTests.Merge;

public class MergerTests
{

    [ModuleInitializer]
    internal static void Init() => VerifierSettings.UseStrictJson();
    
    [Fact]
    public Task When_MergingJsonObjects_Then_ResultShouldBeTheCombinationOfBoth()
    {
        // arrange
        var merger = new Merger();
        var appSetting = JsonConvert.SerializeObject(new
        {
            key1 = "correct",
            key2 = "incorrect"
        });
        var environmentSetting = JsonConvert.SerializeObject(new
        {
            key2 = "correct",
            key3 = "correct"
        });
            
        
        // act
        var result = merger.Merge(appSetting, environmentSetting);
        
        // assert
        return VerifyJson(result);
    }
    
    [Fact]
    public Task When_MergingComplexJsonObjects_Then_ResultShouldBeTheCombinationOfBoth001()
    {
        // arrange
        var merger = new Merger();
        var appSetting = JsonConvert.SerializeObject(new
        {
            key1 = "correct",
            key2 = "incorrect",
            key4 = new
            {
                key5 = "correct"
            }
        });
        var environmentSetting = JsonConvert.SerializeObject(new
        {
            key2 = "correct",
            key3 = "correct",
            key4 = new
            {
                key6 = "correct"
            }
        });
            
        
        // act
        var result = merger.Merge(appSetting, environmentSetting);
        
        // assert
        return VerifyJson(result);
    }
    
    [Fact]
    public Task When_MergingComplexJsonObjects_Then_ResultShouldBeTheCombinationOfBoth002()
    {
        // arrange
        var merger = new Merger();
        var appSetting = JsonConvert.SerializeObject(new
        {
            name = "incorrect",
            age = "correct",
            address = new
            {
                street = "incorrect",
                city = "correct"
            }
        });
        var environmentSetting = JsonConvert.SerializeObject(new
        {
            name = "correct",
            address = new
            {
                street = "correct"
            },
            likes = "correct"
        });
            
        
        // act
        var result = merger.Merge(appSetting, environmentSetting);
        
        // assert
        return VerifyJson(result);
    }
    
    [Fact]
    public void When_MergingInvalidJson_Then_ShouldThrowException()
    {
        // arrange
        var merger = new Merger();
        const string appSetting = "invalid json";
        var environmentSetting = JsonConvert.SerializeObject(new
        {
            key2 = "value3",
            key3 = "value4"
        });
            
        
        // act
        var exception = Record.Exception(() => merger.Merge(appSetting, environmentSetting));
        
        // assert
        exception.Should().NotBeNull();
        exception.Should().BeOfType<JsonReaderException>();
    }
}