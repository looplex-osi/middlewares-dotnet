using System.Dynamic;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.ExtensionMethods;

[TestClass]
public class ResourceTypeRoutesExtensionMethodsTests
{
    [TestMethod]
    public async Task GetMiddleware_ShouldInvokeGetAllAsyncAndWriteResponse()
    {
        // Arrange
        var schemaService = Substitute.For<IResourceTypeService>();
        var context = Substitute.For<IScimV2Context>();
        dynamic state = new ExpandoObject();
        context.State.Returns(state);
        var services = Substitute.For<IServiceProvider>();
        context.State.HttpContext = new DefaultHttpContext();
        context.State.HttpContext.RequestServices = services;
        services.GetService(typeof(IResourceTypeService)).Returns(schemaService);

        // Mock response content
        context.Result = "mockResult";

        // Act
        var middleware = ResourceTypeRoutesExtensionMethods.GetMiddleware();
        await middleware(context, CancellationToken.None, () => Task.CompletedTask);

        // Assert
        await schemaService.Received(1).GetAllAsync(context, CancellationToken.None);
    }

    [TestMethod]
    public async Task GetByIdMiddleware_ShouldInvokeGetByIdAsyncAndWriteResponse()
    {
        // Arrange
        var schemaService = Substitute.For<IResourceTypeService>();
        var context = Substitute.For<IScimV2Context>();
        dynamic state = new ExpandoObject();
        context.State.Returns(state);
        var services = Substitute.For<IServiceProvider>();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["resourceTypeId"] = "mockId"; // Mock RouteValues
        httpContext.RequestServices = services;
        context.State.HttpContext = httpContext;
        services.GetService(typeof(IResourceTypeService)).Returns(schemaService);

        // Mock response content
        context.Result = "mockResult";

        // Act
        var middleware = ResourceTypeRoutesExtensionMethods.GetByIdMiddleware();
        await middleware(context, CancellationToken.None, () => Task.CompletedTask);

        // Assert
        await schemaService.Received(1).GetByIdAsync(context, CancellationToken.None);
    }
}