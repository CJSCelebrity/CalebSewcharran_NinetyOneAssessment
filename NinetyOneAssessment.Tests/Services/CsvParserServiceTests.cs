using NinetyOneAssessment.Application.Services;

namespace NinetyOneAssessment.Tests.Services;

public class CsvParserServiceTests
{
    private readonly CsvParserService _sut = new();

    [Fact]
    public void Parse_SimpleRows_ShouldReturnRowsAndFields()
    {
        //Arrange
        var result = _sut.Parse("a,b\nc,d");
        
        //Act & Assert
        result.Count.ShouldBe(2);
        result[0].Fields.ShouldBe(new[] { "a", "b" });
        result[1].Fields.ShouldBe(new[] { "c", "d" });
    }

    [Fact]
    public void Parse_QuotedFieldContainingComma_ShouldReturnSingleField()
    {
        //Arrange
        var result  = _sut.Parse("\"John, Doe\",42");
        
        //Act & Assert
        result[0].Fields.Count().ShouldBe(2);
        result[0].Fields[0].ShouldBe("John, Doe");
    }

    [Theory]
    [InlineData("a,b\r\nc,d")]
    [InlineData("a,b\nc,d")]
    [InlineData("a,b\r\nc,d\n")]
    public void Parse_MixedLineEndings_ShouldReturnTwoRows(string rowWithMixedLine)
    {
        _sut.Parse(rowWithMixedLine).Count.ShouldBe(2);
    }
}