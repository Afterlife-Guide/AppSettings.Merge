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
        // Arrange
        var merger = new Merger();
        var appSetting = JsonConvert.SerializeObject(new
        {
            key1 = "value1",
            key2 = "value2"
        });
        var environmentSetting = JsonConvert.SerializeObject(new
        {
            key2 = "value3",
            key3 = "value4"
        });
            
        
        // Act
        var result = merger.Merge(appSetting, environmentSetting);
        
        // Assert
        return VerifyJson(result);
    }
    
    [Fact]
    public Task When_MergingComplexJsonObjects_Then_ResultShouldBeTheCombinationOfBoth()
    {
        // Arrange
        var merger = new Merger();
        var appSetting = JsonConvert.SerializeObject(new
        {
            key1 = "value1",
            key2 = "value2",
            key4 = new
            {
                key5 = "value5"
            }
        });
        var environmentSetting = JsonConvert.SerializeObject(new
        {
            key2 = "value3",
            key3 = "value4",
            key4 = new
            {
                key6 = "value6"
            }
        });
            
        
        // Act
        var result = merger.Merge(appSetting, environmentSetting);
        
        // Assert
        return VerifyJson(result);
    }
    
    [Fact]
    public void When_MergingInvalidJson_Then_ShouldThrowException()
    {
        // Arrange
        var merger = new Merger();
        const string appSetting = "invalid json";
        var environmentSetting = JsonConvert.SerializeObject(new
        {
            key2 = "value3",
            key3 = "value4"
        });
            
        
        // Act
        var exception = Record.Exception(() => merger.Merge(appSetting, environmentSetting));
        
        // Assert
        exception.Should().NotBeNull();
        exception.Should().BeOfType<JsonReaderException>();
    }
}