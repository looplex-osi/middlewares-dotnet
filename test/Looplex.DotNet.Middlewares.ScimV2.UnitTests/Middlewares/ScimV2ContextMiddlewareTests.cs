using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Middlewares;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Looplex.DotNet.Core.Application.Abstractions.Providers;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Middlewares;

[TestClass]
public class ScimV2ContextMiddlewareTests
{
    private IScimV2Context _context = null!;
    private Func<Task> _next = null!;
    private IServiceProvider _services = null!;
    private ISqlDatabaseProvider _sqlDatabaseProvider = null!;
    
    [TestInitialize]
    public void Setup()
    {
        // Mock dependencies
        var httpContext = Substitute.For<HttpContext>();
        var httpRequest = Substitute.For<HttpRequest>();
        var routeValues = new RouteValueDictionary { { "key1", "value1" } };
        var queryCollection = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "query1", "value1" }
        });
        var headers = new HeaderDictionary
        {
            { "header1", "value1" }
        };
        httpRequest.RouteValues.Returns(routeValues);
        httpRequest.Query.Returns(queryCollection);
        httpRequest.Headers.Returns(headers);
        httpContext.Request.Returns(httpRequest);
        
        _services = Substitute.For<IServiceProvider>();
        _sqlDatabaseProvider = Substitute.For<ISqlDatabaseProvider>();
        _context = new DefaultScimV2Context(_services, _sqlDatabaseProvider);
        _context.State.HttpContext = httpContext;
        
        // Set up the next middleware delegate
        _next = Substitute.For<Func<Task>>();
    }

    [TestMethod]
    public async Task ScimV2ContextMiddleware_ShouldMapValuesToContext()
    {
        // Act
        await ScimV2Middlewares.ScimV2ContextMiddleware(_context, CancellationToken.None, _next);
        
        // Assert
        Assert.AreEqual(1, _context.RouteValues.Count);
        Assert.AreEqual("value1", _context.RouteValues["key1"]);

        Assert.AreEqual(1, _context.Query.Count);
        Assert.AreEqual("value1", _context.Query["query1"]);

        Assert.AreEqual(1, _context.Headers.Count);
        Assert.AreEqual("value1", _context.Headers["header1"]);
    }
}