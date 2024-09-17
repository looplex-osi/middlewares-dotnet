using System.Dynamic;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Dtos;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Factories;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Domain;
using Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.OAuth2.UnitTests.ExtensionMethods;

[TestClass]
public class TokenRoutesExtensionMethodsTests
{
    [TestMethod]
    public async Task TokenMiddleware_ShouldInvokeAuthorizationServiceAndWriteResponse()
    {
        // Arrange
        var authorizationServiceFactory = Substitute.For<IAuthorizationServiceFactory>();
        var authorizationService = Substitute.For<IAuthorizationService>();
        authorizationServiceFactory.GetService(Arg.Any<GrantType>()).Returns(authorizationService);
        var context = Substitute.For<IContext>();
        var services = Substitute.For<IServiceProvider>();
        context.Services.Returns(services);
        dynamic state = new ExpandoObject();
        context.State.Returns(state);
        services.GetService(typeof(IAuthorizationService)).Returns(authorizationService);
        services.GetService(typeof(IAuthorizationServiceFactory)).Returns(authorizationServiceFactory);

        // Setup HttpContext with necessary headers and request
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
        {
            { "grant_type", "urn:ietf:params:oauth:grant-type:token-exchange" },
            { "subject_token", "test-token" },
            { "subject_token_type", "urn:ietf:params:oauth:token-type:access_token" },
        });
        state.HttpContext = httpContext;
        
        // Act
        await TokenRoutesExtensionMethods.TokenMiddleware(context, CancellationToken.None, () => Task.CompletedTask);

        // Assert
        var clientCredentialsDto = JsonConvert.DeserializeObject<ClientCredentialsDto>((string)context.State.Resource)!;

        Assert.AreEqual("urn:ietf:params:oauth:grant-type:token-exchange", clientCredentialsDto.GrantType);
        Assert.AreEqual("test-token", clientCredentialsDto.SubjectToken);
        Assert.AreEqual("urn:ietf:params:oauth:token-type:access_token", clientCredentialsDto.SubjectTokenType);

        await authorizationService.Received(1).CreateAccessToken(context, CancellationToken.None);
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task TokenMiddleware_ShouldThrowException_WhenRequestBodyIsJson()
    {
        // Arrange
        var authorizationService = Substitute.For<IAuthorizationService>();
        var context = Substitute.For<IContext>();
        var services = Substitute.For<IServiceProvider>();
        context.Services.Returns(services);
        dynamic state = new ExpandoObject();
        context.State.Returns(state);
        services.GetService(typeof(IAuthorizationService)).Returns(authorizationService);

        // Setup HttpContext with necessary headers and request
        var httpContext = new DefaultHttpContext();
        httpContext.Request.ContentType = "application/json";
        httpContext.Request.Body = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes("{\"key\":\"value\"}"));
        state.HttpContext = httpContext;

        // Act
        // The following call should throw InvalidOperationException
        await TokenRoutesExtensionMethods.TokenMiddleware(context, CancellationToken.None, () => Task.CompletedTask);

        // Assert
        // The test expects an InvalidOperationException, so no assertions needed.
    }
}