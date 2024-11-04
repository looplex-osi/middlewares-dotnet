using System.Dynamic;
using System.Text;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.ExtensionMethods;

[TestClass]
public class BulkRoutesExtensionMethodsTests
{
    private IContext _context = null!;
    private IBulkService _bulkService = null!;
    private IServiceProvider _serviceProvider = null!;
    private CancellationToken _cancellationToken;

    [TestInitialize]
    public void Setup()
    {
        _context = Substitute.For<IContext>();
        _bulkService = Substitute.For<IBulkService>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _cancellationToken = CancellationToken.None;

        // Set up the mock HttpContext and dependency injection
        dynamic state = new ExpandoObject();
        _context.State.Returns(state);
        var httpContext = new DefaultHttpContext
        {
            RequestServices = _serviceProvider
        };
        _context.State.HttpContext = httpContext;
        _serviceProvider.GetService(typeof(IBulkService)).Returns(_bulkService);
    }

    [TestMethod]
    public async Task PostMiddleware_ShouldReadRequestBody_SetIdAndCallBulkService()
    {
        // Arrange
        var requestBody = "{\"operation\": \"test\"}";
        var id = "mockId";
        var httpContext = (DefaultHttpContext)_context.State.HttpContext;

        // Set up the request body
        httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

        // Set up route values
        httpContext.Request.RouteValues["id"] = id;

        // Mock result
        _context.Result = "mockResult";

        // Act
        var middleware = BulkRoutesExtensionMethods.PostMiddleware();
        await middleware(_context, _cancellationToken, () => Task.CompletedTask);

        // Assert
        Assert.AreEqual(requestBody, _context.State.Resource);
        Assert.AreEqual(id, _context.State.Id);
        await _bulkService.Received(1).ExecuteBulkOperationsAsync(_context, _cancellationToken);
    }
}