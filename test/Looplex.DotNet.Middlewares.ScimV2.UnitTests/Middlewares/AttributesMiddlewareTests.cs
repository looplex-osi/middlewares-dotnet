using Looplex.DotNet.Core.Application.Abstractions.Providers;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Middlewares.ScimV2.Middlewares;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Middlewares;

[TestClass]
public class AttributesMiddlewareTests
{
    private IScimV2Context _context = null!;

    [TestInitialize]
    public void Setup()
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        var sqlDatabaseProvider = Substitute.For<ISqlDatabaseProvider>();
        _context = new DefaultScimV2Context(serviceProvider, sqlDatabaseProvider);
    }

    [TestMethod]
    public async Task AttributesMiddleware_ShouldExcludeAttributes()
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
        _context.Result = JsonConvert.SerializeObject(originalJson);
        _context.Query.Add("excludedAttributes", "age,addresses[*].street,addresses[0].number");

        // Act
        await ScimV2Middlewares.AttributesMiddleware(_context, CancellationToken.None, () => Task.CompletedTask);

        // Assert
        var resultJson = JObject.Parse((string)_context.Result!);
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
    public async Task AttributesMiddleware_ShouldThrowException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        _context.Result = "{ \"name\": \"John\" }";
        _context.Query.Add("attributes", "name,email"); 

        // Act & Assert
        await ScimV2Middlewares.AttributesMiddleware(_context, cancellationToken, () => Task.CompletedTask);
    }

    [TestMethod]
    [ExpectedException(typeof(OperationCanceledException))]
    public async Task AttributesMiddleware_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        _context.Result = "{ \"name\": \"John\" }";
        _context.Query.Add("attributes", "name,email"); 
        _context.Query.Add("excludedAttributes", string.Empty);

        // Act & Assert
        await ScimV2Middlewares.AttributesMiddleware(_context, cts.Token, () => Task.CompletedTask);
    }
}