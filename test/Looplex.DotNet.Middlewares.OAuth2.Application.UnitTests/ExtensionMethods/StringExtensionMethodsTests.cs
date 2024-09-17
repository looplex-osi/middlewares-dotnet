using Looplex.DotNet.Middlewares.OAuth2.Application.ExtensionMethods;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.UnitTests.ExtensionMethods;

[TestClass]
public class StringExtensionMethodsTests
{
    [TestMethod]
    public void IsBasicAuthentication_ValidBasicToken_ReturnsTrueAndToken()
    {
        // Arrange
        var basicAuthHeader = "Basic dG9rZW4=";

        // Act
        var result = basicAuthHeader.IsBasicAuthentication(out var token);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("dG9rZW4=", token);
    }

    [TestMethod]
    public void IsBasicAuthentication_InvalidBasicToken_ReturnsFalseAndNullToken()
    {
        // Arrange
        var invalidAuthHeader = "NotBasic dG9rZW4=";

        // Act
        var result = invalidAuthHeader.IsBasicAuthentication(out var token);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(token);
    }

    [TestMethod]
    public void IsBearerAuthentication_ValidBearerToken_ReturnsTrueAndToken()
    {
        // Arrange
        var bearerAuthHeader = "Bearer abcdef123456";

        // Act
        var result = bearerAuthHeader.IsBearerAuthentication(out var token);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abcdef123456", token);
    }

    [TestMethod]
    public void IsBearerAuthentication_InvalidBearerToken_ReturnsFalseAndNullToken()
    {
        // Arrange
        var invalidAuthHeader = "NotBearer abcdef123456";

        // Act
        var result = invalidAuthHeader.IsBearerAuthentication(out var token);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(token);
    }

    [TestMethod]
    public void IsBasicAuthentication_EmptyString_ReturnsFalseAndNullToken()
    {
        // Arrange
        var emptyAuthHeader = string.Empty;

        // Act
        var result = emptyAuthHeader.IsBasicAuthentication(out var token);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(token);
    }

    [TestMethod]
    public void IsBearerAuthentication_EmptyString_ReturnsFalseAndNullToken()
    {
        // Arrange
        var emptyAuthHeader = string.Empty;

        // Act
        var result = emptyAuthHeader.IsBearerAuthentication(out var token);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(token);
    }

    [TestMethod]
    public void IsBasicAuthentication_CaseInsensitive_ReturnsTrueAndToken()
    {
        // Arrange
        var mixedCaseAuthHeader = "bAsIc dG9rZW4=";

        // Act
        var result = mixedCaseAuthHeader.IsBasicAuthentication(out var token);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("dG9rZW4=", token);
    }

    [TestMethod]
    public void IsBearerAuthentication_CaseInsensitive_ReturnsTrueAndToken()
    {
        // Arrange
        var mixedCaseAuthHeader = "bEaReR abcdef123456";

        // Act
        var result = mixedCaseAuthHeader.IsBearerAuthentication(out var token);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abcdef123456", token);
    }
}