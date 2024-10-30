using Looplex.DotNet.Middlewares.ScimV2.Utils;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Utils;
    
[TestClass]
public class PaginationUtilsTests
{
    [TestMethod]
    public void CreateLinkHeader_ShouldReturnCorrectSelfAndFirstLinks()
    {
        // Arrange
        var baseUri = new Uri("https://example.com:443/resources?count=10");
        int startIndex = 1;
        int count = 10;
        int totalResults = 50;

        // Act
        var result = PaginationUtils.CreateLinkHeader(baseUri, startIndex, count, totalResults);

        // Assert
        StringAssert.Contains(result, "<https://example.com:443/resources?count=10&startIndex=1>; rel=\"self\"");
        StringAssert.Contains(result, "<https://example.com:443/resources?count=10&startIndex=1>; rel=\"first\"");
    }

    [TestMethod]
    public void CreateLinkHeader_ShouldIncludePrevLink_WhenNotFirstPage()
    {
        // Arrange
        var baseUri = new Uri("https://example.com:443/resources?count=10");
        int startIndex = 11;
        int count = 10;
        int totalResults = 50;

        // Act
        var result = PaginationUtils.CreateLinkHeader(baseUri, startIndex, count, totalResults);

        // Assert
        StringAssert.Contains(result, "<https://example.com:443/resources?count=10&startIndex=1>; rel=\"prev\"");
    }

    [TestMethod]
    public void CreateLinkHeader_ShouldIncludeNextLink_WhenNotLastPage()
    {
        // Arrange
        var baseUri = new Uri("https://example.com:443/resources?count=10");
        int startIndex = 11;
        int count = 10;
        int totalResults = 50;

        // Act
        var result = PaginationUtils.CreateLinkHeader(baseUri, startIndex, count, totalResults);

        // Assert
        StringAssert.Contains(result, "<https://example.com:443/resources?count=10&startIndex=21>; rel=\"next\"");
    }

    [TestMethod]
    public void CreateLinkHeader_ShouldIncludeLastLink_WhenNotLastPage()
    {
        // Arrange
        var baseUri = new Uri("https://example.com:443/resources?count=10");
        int startIndex = 11;
        int count = 10;
        int totalResults = 50;

        // Act
        var result = PaginationUtils.CreateLinkHeader(baseUri, startIndex, count, totalResults);

        // Assert
        StringAssert.Contains(result, "<https://example.com:443/resources?count=10&startIndex=41>; rel=\"last\"");
    }

    [TestMethod]
    public void CreateLinkHeader_ShouldReturnCorrectLinksForLastPage()
    {
        // Arrange
        var baseUri = new Uri("https://example.com:443/resources?count=10");
        int startIndex = 31;
        int count = 10;
        int totalResults = 50;

        // Act
        var result = PaginationUtils.CreateLinkHeader(baseUri, startIndex, count, totalResults);

        // Assert
        StringAssert.Contains(result, "<https://example.com:443/resources?count=10&startIndex=31>; rel=\"self\"");
        StringAssert.Contains(result, "<https://example.com:443/resources?count=10&startIndex=1>; rel=\"first\"");
        StringAssert.Contains(result, "<https://example.com:443/resources?count=10&startIndex=21>; rel=\"prev\"");
        StringAssert.Contains(result, "<https://example.com:443/resources?count=10&startIndex=41>; rel=\"last\"");
    }
}