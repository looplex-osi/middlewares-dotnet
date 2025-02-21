using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.UnitTests.ExtensionMethods;

[TestClass]
public class ContextExtensionMethodsTests
{
    [TestMethod]
    public void AsScimV2Context_ValidContext_ReturnsScimV2Context()
    {
        // Arrange
        var context = Substitute.For<IContext, IScimV2Context>();

        // Act
        var scimV2Context = context.AsScimV2Context();

        // Assert
        Assert.IsNotNull(scimV2Context);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void AsScimV2Context_InvalidContext_ThrowsException()
    {
        // Arrange
        var context = Substitute.For<IContext>();

        // Act
        context.AsScimV2Context();
    }

    [TestMethod]
    public void GetRouteValue_ValidKey_ReturnsValue()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.RouteValues.Returns(new Dictionary<string, object?>
        {
            { "key", 42 }
        });

        var context = (IContext)scimV2Context;

        // Act
        var value = context.GetRouteValue<int>("key");

        // Assert
        Assert.AreEqual(42, value);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void GetRouteValue_InvalidType_ThrowsException()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.RouteValues.Returns(new Dictionary<string, object?>
        {
            { "key", "value" }
        });

        var context = (IContext)scimV2Context;

        // Act
        context.GetRouteValue<int>("key");
    }

    [TestMethod]
    public void GetRequiredRouteValue_ValidKey_ReturnsValue()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.RouteValues.Returns(new Dictionary<string, object?>
        {
            { "key", 42 }
        });

        var context = (IContext)scimV2Context;

        // Act
        var value = context.GetRequiredRouteValue<int>("key");

        // Assert
        Assert.AreEqual(42, value);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void GetRequiredRouteValue_InvalidCastValue_ThrowsException()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.RouteValues.Returns(new Dictionary<string, object?>
        {
            { "key", null }
        });

        var context = (IContext)scimV2Context;

        // Act
        context.GetRequiredRouteValue<int>("key");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void GetRequiredRouteValue_NullValue_ThrowsException()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.RouteValues.Returns(new Dictionary<string, object?>
        {
            { "key", null as int? }
        });

        var context = (IContext)scimV2Context;

        // Act
        context.GetRequiredRouteValue<int>("key");
    }

    [TestMethod]
    public void GetQuery_ValidKey_ReturnsValue()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.Query.Returns(new Dictionary<string, string>
        {
            { "key", "value" }
        });

        var context = (IContext)scimV2Context;

        // Act
        var value = context.GetQuery("key");

        // Assert
        Assert.AreEqual("value", value);
    }

    [TestMethod]
    public void GetQuery_InvalidKey_ReturnsNull()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.Query.Returns(new Dictionary<string, string>());

        var context = (IContext)scimV2Context;

        // Act
        var value = context.GetQuery("missingKey");

        // Assert
        Assert.IsNull(value);
    }

    [TestMethod]
    public void GetRequiredQuery_ValidKey_ReturnsValue()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.Query.Returns(new Dictionary<string, string>
        {
            { "key", "value" }
        });

        var context = (IContext)scimV2Context;

        // Act
        var value = context.GetRequiredQuery("key");

        // Assert
        Assert.AreEqual("value", value);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void GetRequiredQuery_MissingKey_ThrowsException()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.Query.Returns(new Dictionary<string, string>());

        var context = (IContext)scimV2Context;

        // Act
        context.GetRequiredQuery("missingKey");
    }

    [TestMethod]
    public void GetHeader_ValidKey_ReturnsValue()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.Headers.Returns(new Dictionary<string, string>
        {
            { "key", "value" }
        });

        var context = (IContext)scimV2Context;

        // Act
        var value = context.GetHeader("key");

        // Assert
        Assert.AreEqual("value", value);
    }

    [TestMethod]
    public void GetHeader_InvalidKey_ReturnsNull()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.Headers.Returns(new Dictionary<string, string>());

        var context = (IContext)scimV2Context;

        // Act
        var value = context.GetHeader("missingKey");

        // Assert
        Assert.IsNull(value);
    }

    [TestMethod]
    public void GetRequiredHeader_ValidKey_ReturnsValue()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.Headers.Returns(new Dictionary<string, string>
        {
            { "key", "value" }
        });

        var context = (IContext)scimV2Context;

        // Act
        var value = context.GetRequiredHeader("key");

        // Assert
        Assert.AreEqual("value", value);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void GetRequiredHeader_MissingKey_ThrowsException()
    {
        // Arrange
        var scimV2Context = Substitute.For<IScimV2Context>();
        scimV2Context.Headers.Returns(new Dictionary<string, string>());

        var context = (IContext)scimV2Context;

        // Act
        context.GetRequiredHeader("missingKey");
    }
    
    [TestMethod]
    public void AttributesMiddleware_ShouldExcludeAttributes()
    {
        // Arrange
        var originalJson = new
        {
            name = "John",
            age = 30,
            email = "john@example.com",
            addresses = new List<object>
            {
                new
                {
                    street = "street 1",
                    number = "234"
                },
                new
                {
                    street = "street 1",
                    number = "234"
                }
            }
        };
        var serviceProvider = Substitute.For<IServiceProvider>();
        IContext context = new DefaultScimV2Context(serviceProvider);
        context.Result = JsonConvert.SerializeObject(originalJson);
        context.AsScimV2Context().Query.Add("excludedAttributes", "age,addresses[*].street,addresses[0].number");

        // Act
        var result = context.GetResult();
        
        // Assert
        var resultJson = JObject.Parse((string)result!);
        Assert.IsNotNull(resultJson["name"]);
        Assert.IsNotNull(resultJson["email"]);
        Assert.IsNull(resultJson["age"]);
        Assert.IsNotNull(resultJson["addresses"] != null);
        Assert.IsNull(resultJson["addresses"]![0]!["number"]);
        Assert.IsNotNull(resultJson["addresses"]![1]!["number"]);
        Assert.IsNull(resultJson["addresses"]![0]!["street"]);
        Assert.IsNull(resultJson["addresses"]![1]!["street"]);
    }

    [TestMethod]
    [ExpectedException(typeof(Error))]
    public void AttributesMiddleware_ShouldThrowException()
    {
        // Arrange

        var serviceProvider = Substitute.For<IServiceProvider>();
        IContext context = new DefaultScimV2Context(serviceProvider);
        context.Result = "{ \"name\": \"John\" }";
        context.AsScimV2Context().Query.Add("attributes", "name,email"); 

        // Act & Assert
        context.GetResult();
    }
}