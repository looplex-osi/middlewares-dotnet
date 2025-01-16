using System.Dynamic;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.ExtensionMethods;

[TestClass]
public class SchemaRoutesExtensionMethodsTests
{
    [TestMethod]
    public async Task GetMiddleware_ShouldInvokeGetAllAsyncAndWriteResponse()
    {
        // Arrange
        var schemaService = Substitute.For<ISchemaService>();
        var context = Substitute.For<IScimV2Context>();
        dynamic state = new ExpandoObject();
        context.State.Returns(state);
        var services = Substitute.For<IServiceProvider>();
        context.State.HttpContext = new DefaultHttpContext();
        context.State.HttpContext.RequestServices = services;
        context.State.CancellationToken = CancellationToken.None;
        services.GetService(typeof(ISchemaService)).Returns(schemaService);

        // Mock response content
        context.Result = "mockResult";

        // Act
        var middleware = SchemaRoutesExtensionMethods.GetMiddleware();
        await middleware(context, () => Task.CompletedTask);

        // Assert
        await schemaService.Received(1).GetAllAsync(context);
    }

    [TestMethod]
    public async Task GetByIdMiddleware_ShouldInvokeGetByIdAsyncAndWriteResponse()
    {
        // Arrange
        var schemaService = Substitute.For<ISchemaService>();
        var context = Substitute.For<IScimV2Context>();
        dynamic state = new ExpandoObject();
        context.State.Returns(state);
        var services = Substitute.For<IServiceProvider>();
        var httpContext = new DefaultHttpContext();
        context.RouteValues = new Dictionary<string, object?>
        {
            { "SchemaId", "mockId" }
        };
        httpContext.Request.RouteValues["SchemaId"] = "mockId"; // Mock RouteValues
        httpContext.RequestServices = services;
        context.State.HttpContext = httpContext;
        context.State.CancellationToken = CancellationToken.None;
        services.GetService(typeof(ISchemaService)).Returns(schemaService);

        // Mock response content
        context.Result = "mockResult";

        // Act
        var middleware = SchemaRoutesExtensionMethods.GetByIdMiddleware();
        await middleware(context, () => Task.CompletedTask);

        // Assert
        await schemaService.Received(1).GetByIdAsync(context);
    }
}