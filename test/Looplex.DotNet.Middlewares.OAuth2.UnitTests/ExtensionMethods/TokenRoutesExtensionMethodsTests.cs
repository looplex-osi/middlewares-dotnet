using System.Text;
using System.Text.Json;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.OpenForExtension.Context;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;

namespace Looplex.DotNet.Middlewares.OAuth2.UnitTests.ExtensionMethods;

[TestClass]
public class ClientRoutesExtensionMethodsTests
{
    [TestMethod]
    public async Task TokenMiddleware_ShouldProcessRequest()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();

        var context = DefaultContext.Create([], serviceProvider);
        var httpContext = new DefaultHttpContext();
        var authorizationService = Substitute.For<IAuthorizationService>();
        
        serviceProvider.GetService(typeof(IAuthorizationService)).Returns(authorizationService);
        context.State.HttpContext = httpContext;

        httpContext.Request.Headers.Authorization = "Bearer sample-token";
        var requestBody = "{ \"key\": \"value\" }";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
        httpContext.Request.Body = stream;
        httpContext.Response.Body = new MemoryStream(); 
        
        context.State.Authorization = null;
        context.State.Resource = null;
        context.Result = new { result = "success" }; 

        // Act
        await TokenRoutesExtensionMethods.TokenMiddleware(context, CancellationToken.None, null);

        // Assert
        Assert.AreEqual("Bearer sample-token", context.State.Authorization);
        Assert.AreEqual(requestBody, context.State.Resource);
        
        // Validate the response
        stream.Position = 0;
        using (var reader = new StreamReader(httpContext.Response.Body))
        {
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var jsonResponse = await reader.ReadToEndAsync();
            var expectedResponse = JsonSerializer.Serialize(context.Result);
            Assert.AreEqual(expectedResponse, jsonResponse);
        }

        await authorizationService.Received().CreateAccessToken(context);
    }
}