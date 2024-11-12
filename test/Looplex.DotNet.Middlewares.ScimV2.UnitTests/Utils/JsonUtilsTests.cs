using Looplex.DotNet.Middlewares.ScimV2.Utils;
using Newtonsoft.Json.Linq;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Utils;

[TestClass]
public class JsonUtilsTests
{
    [TestMethod]
    public void Traverse_ShouldApplyVisitorToSingleToken()
    {
        // Arrange
        var token = JToken.Parse("\"value\"");
        var visitedTokens = new List<JToken>();
        Action<JToken> visitor = t => visitedTokens.Add(t);

        // Act
        JsonUtils.Traverse(token, visitor);

        // Assert
        Assert.AreEqual(1, visitedTokens.Count);
        Assert.AreEqual("value", visitedTokens[0].Value<string>());
    }

    [TestMethod]
    public void Traverse_ShouldApplyVisitorToAllTokensInNestedObject()
    {
        // Arrange
        var json = JToken.Parse("{ \"prop1\": \"value1\", \"prop2\": { \"subProp\": \"value2\" } }");
        var visitedTokens = new List<JToken>();
        Action<JToken> visitor = t => visitedTokens.Add(t);

        // Act
        JsonUtils.Traverse(json, visitor);

        // Assert
        Assert.AreEqual(7, visitedTokens.Count); // root, prop1, "value1", prop2, subProp "value2"
        Assert.AreEqual("value1", ((JProperty)visitedTokens[1]).Value.Value<string>());
        Assert.AreEqual("value2", ((JProperty)visitedTokens[5]).Value.Value<string>());
        
    }

    [TestMethod]
    public void Traverse_ShouldApplyVisitorToAllTokensInArray()
    {
        // Arrange
        var json = JToken.Parse("[ { \"prop1\": \"value1\" }, { \"prop2\": \"value2\" } ]");
        var visitedTokens = new List<JToken>();
        Action<JToken> visitor = t => visitedTokens.Add(t);

        // Act
        JsonUtils.Traverse(json, visitor);

        // Assert
        Assert.AreEqual(7, visitedTokens.Count); // root array, first object, prop1:"value1", second object, prop2:"value2"
        Assert.AreEqual("value1", ((JProperty)visitedTokens[2]).Value.Value<string>());
        Assert.AreEqual("value2", ((JProperty)visitedTokens[5]).Value.Value<string>());
    }

    [TestMethod]
    public void Traverse_ShouldApplyVisitorToComplexNestedStructure()
    {
        // Arrange
        var json = JToken.Parse("{ \"array\": [ { \"prop1\": \"value1\" }, { \"prop2\": \"value2\" } ], \"simpleProp\": \"simpleValue\" }");
        var visitedTokens = new List<JToken>();
        Action<JToken> visitor = t => visitedTokens.Add(t);

        // Act
        JsonUtils.Traverse(json, visitor);

        // Assert
        Assert.AreEqual(11, visitedTokens.Count);
        Assert.AreEqual("value1", ((JProperty)visitedTokens[4]).Value.Value<string>());
        Assert.AreEqual("value2", ((JProperty)visitedTokens[7]).Value.Value<string>());
        Assert.AreEqual("simpleValue", ((JProperty)visitedTokens[9]).Value.Value<string>());
    }
}