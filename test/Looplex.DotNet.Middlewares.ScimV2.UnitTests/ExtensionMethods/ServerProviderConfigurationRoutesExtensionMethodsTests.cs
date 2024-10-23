using System.Dynamic;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.ExtensionMethods;

[TestClass]
public class ServerProviderConfigurationRoutesExtensionMethodsTests
{
    [TestMethod]
    public async Task GetMiddleware_NoExceptionIsThrown()
    {
        // Arrange
        var serviceProviderConfiguration = new ServiceProviderConfiguration
        {
            Bulk = new(),
            ChangePassword = new(),
            Filter = new(),
            Patch = new(),
            Sort = new(),
            Etag = new(),
            AuthenticationSchemes = []
        };
        var context = Substitute.For<IContext>();
        dynamic state = new ExpandoObject();
        context.State.Returns(state);
        var services = Substitute.For<IServiceProvider>();
        context.State.HttpContext = new DefaultHttpContext();
        context.State.HttpContext.RequestServices = services;
        services.GetService(typeof(ServiceProviderConfiguration)).Returns(serviceProviderConfiguration);

        // Act & Assert
        var middleware = ServerProviderConfigurationRoutesExtensionMethods.GetMiddleware();
        await middleware(context, CancellationToken.None, () => Task.CompletedTask);
    }
}