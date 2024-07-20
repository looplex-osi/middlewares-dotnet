using System.Net;
using System.Text;
using System.Text.Json;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Middlewares.Clients.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Dtos;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;
using Looplex.OpenForExtension.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.UnitTests.Services;

[TestClass]
public class AuthorizationServiceTests
{
    private IConfiguration _mockConfiguration = null!;
    private IClientService _mockClientService = null!;
    private IServiceProvider _mockServiceProvider = null!;
    private IContextFactory _mockContextFactory = null!;
    private DefaultHttpContext _httpContext = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockConfiguration = Substitute.For<IConfiguration>();
        _mockClientService = Substitute.For<IClientService>();
        _mockServiceProvider = Substitute.For<IServiceProvider>();
        _mockContextFactory = Substitute.For<IContextFactory>();
        _httpContext = new DefaultHttpContext();

        var configurationSection = Substitute.For<IConfigurationSection>();
        configurationSection.Value.Returns("20");
        _mockConfiguration.GetSection("TokenExpirationTimeInMinutes").Returns(configurationSection);

        _mockServiceProvider.GetService(typeof(IContextFactory)).Returns(_mockContextFactory);
    }

    [TestMethod]
    public async Task TokenMiddleware_InvalidAuth_ThrowsUnauthorized()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<IIdTokenService>();
        var authorization = "Basic xxxxxx";
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials"",
            ""id_token"": ""validIdToken""
        }";
        
        var context = DefaultContext.Create([], _mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new AuthorizationService(_mockConfiguration, _mockClientService, mockIdTokenService);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context, CancellationToken.None));
        Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
        Assert.AreEqual(exception.Message, "Invalid authorization.");
    }

    [TestMethod]
    public async Task TokenMiddleware_InvalidGrantType_ThrowsUnauthorized()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<IIdTokenService>();
        var authorization = "Bearer xxxxxx";
        var clientCredentials = @"{
            ""grant_type"": ""invalid"",
            ""id_token"": ""validIdToken""
        }";
        
        var context = DefaultContext.Create([], _mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new AuthorizationService(_mockConfiguration, _mockClientService, mockIdTokenService);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context, CancellationToken.None));
        Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
        Assert.AreEqual(exception.Message, "grant_type is invalid.");
    }

    [TestMethod]
    public async Task TokenMiddleware_ValidBearerAdminAuth_ReturnsAccessToken()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<IIdTokenService>();
        string email = "email@looplex.com.br";
        string clientId = Guid.NewGuid().ToString();
        _mockConfiguration["AdminClientId"].Returns(clientId);
        _mockConfiguration["AdminClientSecret"].Returns("clientSecret");
        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["OicdTenantId"].Returns("oicdTenantId");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));

        var authorization = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:clientSecret"));
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials"",
            ""id_token"": ""validIdToken""
        }";

        mockIdTokenService.ValidateIdToken("oicdIssuer", "oicdTenantId", "oicdAudience", "validIdToken", out Arg.Any<string?>()).Returns(x =>
        {
            x[4] = email;
            return true;
        });

        var context = DefaultContext.Create([], _mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new AuthorizationService(_mockConfiguration, _mockClientService, mockIdTokenService);

        // Act
        await service.CreateAccessToken(context, CancellationToken.None);

        // Assert
        Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
        Assert.IsNotNull(context.Result);
        Assert.IsInstanceOfType(context.Result, typeof(AccessTokenDto));
    }

    [TestMethod]
    public async Task TokenMiddleware_InvalidBearerAdminAuthClientSecretIsWrong_ThrowsUnauthorized()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<IIdTokenService>();
        string email = "email@looplex.com.br";
        string clientId = Guid.NewGuid().ToString();
        _mockConfiguration["AdminClientId"].Returns(clientId);
        _mockConfiguration["AdminClientSecret"].Returns("clientSecret");
        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["OicdTenantId"].Returns("oicdTenantId");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));

        var authorization = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:clientSecretWrong"));
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials"",
            ""id_token"": ""validIdToken""
        }";

        mockIdTokenService.ValidateIdToken("oicdIssuer", "oicdTenantId", "oicdAudience", "validIdToken", out Arg.Any<string?>()).Returns(x =>
        {
            x[4] = email;
            return true;
        });
            
        _mockContextFactory.Create(Arg.Any<IEnumerable<string>>()).Returns(DefaultContext.Create(null, null));

        _mockClientService.GetByIdAndSecretOrDefaultAsync(Arg.Any<IDefaultContext>(), CancellationToken.None)
            .Returns(call =>
            {
                var context = call.Arg<IDefaultContext>();
                context.Result = (IClient?)null;
                return Task.CompletedTask;
            });

        var context = DefaultContext.Create([], _mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new AuthorizationService(_mockConfiguration, _mockClientService, mockIdTokenService);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<EntityInvalidException>(() => service.CreateAccessToken(context, CancellationToken.None));
        Assert.AreEqual(exception.ErrorMessages[0], "Invalid clientId or clientSecret.");
    }

    [TestMethod]
    public async Task TokenMiddleware_InvalidBearerAdminAuthClientIdIsWrong_ThrowsUnauthorized()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<IIdTokenService>();
        string email = "email@looplex.com.br";
        string clientId = Guid.NewGuid().ToString();
        string clientIdWrong = Guid.NewGuid().ToString();
        _mockConfiguration["AdminClientId"].Returns(clientId);
        _mockConfiguration["AdminClientSecret"].Returns("clientSecret");
        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["OicdTenantId"].Returns("oicdTenantId");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));

        var authorization = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientIdWrong}:clientSecret"));
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials"",
            ""id_token"": ""validIdToken""
        }";

        mockIdTokenService.ValidateIdToken("oicdIssuer", "oicdTenantId", "oicdAudience", "validIdToken", out Arg.Any<string?>()).Returns(x =>
        {
            x[4] = email;
            return true;
        });

        _mockContextFactory.Create(Arg.Any<IEnumerable<string>>()).Returns(DefaultContext.Create(null, null));
        _mockClientService.GetByIdAndSecretOrDefaultAsync(Arg.Any<IDefaultContext>(), CancellationToken.None)
            .Returns(call =>
            {
                var context = call.Arg<IDefaultContext>();
                context.Result = (IClient?)null;
                return Task.CompletedTask;
            });

        var context = DefaultContext.Create([], _mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new AuthorizationService(_mockConfiguration, _mockClientService, mockIdTokenService);
            
        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<EntityInvalidException>(() => service.CreateAccessToken(context, CancellationToken.None));
        Assert.AreEqual(exception.ErrorMessages[0], "Invalid clientId or clientSecret.");
    }

    [TestMethod]
    public async Task TokenMiddleware_ValidBearerAuth_ReturnsAccessToken()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<IIdTokenService>();
        var email = "test@example.com";
        var clientId = Guid.NewGuid();
        var clientId1 = Guid.NewGuid();
        var clientSecret = "clientSecret";
        var clientCredentialsDto = new ClientCredentialsDto
        {
            IdToken = "validIdToken",
            GrantType = "client_credentials"
        };
        _mockConfiguration["AdminClientId"].Returns(clientId1.ToString());
        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["OicdTenantId"].Returns("oicdTenantId");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));
            
        var authorization = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:clientSecret"));
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials"",
            ""id_token"": ""validIdToken""
        }";

        mockIdTokenService.ValidateIdToken("oicdIssuer", "oicdTenantId", "oicdAudience", "validIdToken", out Arg.Any<string?>()).Returns(x =>
        {
            x[4] = email;
            return true;
        });

        _mockContextFactory.Create(Arg.Any<IEnumerable<string>>()).Returns(DefaultContext.Create(null, null));

        var client = Substitute.For<IClient>();
        client.Id.Returns(clientId.ToString());
        client.DisplayName.Returns("client");
        client.Secret.Returns(clientSecret);
        client.NotBefore.Returns(DateTimeOffset.UtcNow.AddMinutes(-1));
        client.ExpirationTime.Returns(DateTimeOffset.UtcNow.AddMinutes(1));

        _mockClientService.GetByIdAndSecretOrDefaultAsync(
                Arg.Is<IDefaultContext>(c => AssertDefaultContextIsValid(c, clientId, clientSecret)),
                CancellationToken.None)
            .Returns(call =>
            {
                var context = call.Arg<IDefaultContext>();
                context.Result = (IClient?)client;
                return Task.CompletedTask;
            });

        _httpContext.Request.Headers.Authorization = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret));
        _httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clientCredentialsDto)));

        var context = DefaultContext.Create([], _mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new AuthorizationService(_mockConfiguration, _mockClientService, mockIdTokenService);

        // Act
        await service.CreateAccessToken(context, CancellationToken.None);

        // Assert
        Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
        Assert.IsNotNull(context.Result);
        Assert.IsInstanceOfType(context.Result, typeof(AccessTokenDto));
    }

    private bool AssertDefaultContextIsValid(IDefaultContext c, Guid clientId, string clientSecret)
    {
        return c.State.ClientId == clientId
               && c.State.ClientSecret == clientSecret;
    }

    [TestMethod]
    public async Task TokenMiddleware_InvalidBearerAuth_ThrowsUnauthorized()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<IIdTokenService>();
        var clientId = Guid.NewGuid();
        var clientSecret = "clientSecret";
        var clientCredentialsDto = new ClientCredentialsDto
        {
            IdToken = "validIdToken",
            GrantType = "client_credentials"
        };

        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["OicdTenantId"].Returns("oicdTenantId");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));

        var authorization = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:clientSecret"));
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials"",
            ""id_token"": ""validIdToken""
        }";

        mockIdTokenService.ValidateIdToken("oicdIssuer", "oicdTenantId", "oicdAudience", "validIdToken", out Arg.Any<string?>()).Returns(x =>
        {
            x[4] = null;
            return false;
        });


        var client = Substitute.For<IClient>();
        client.Id.Returns(clientId.ToString());
        client.DisplayName.Returns("client");
        client.Secret.Returns(clientSecret);
        client.NotBefore.Returns(DateTimeOffset.UtcNow.AddMinutes(1));
        client.ExpirationTime.Returns(DateTimeOffset.UtcNow.AddMinutes(1));

        _mockClientService.GetByIdAndSecretOrDefaultAsync(
                Arg.Is<IDefaultContext>(c => AssertDefaultContextIsValid(c, clientId, clientSecret)),
                CancellationToken.None)
            .Returns(call =>
            {
                var context = call.Arg<IDefaultContext>();
                context.Result = (IClient?)client;
                return Task.CompletedTask;
            });

        _httpContext.Request.Headers.Authorization = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret));
        _httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clientCredentialsDto)));

        var context = DefaultContext.Create([], _mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new AuthorizationService(_mockConfiguration, _mockClientService, mockIdTokenService);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context, CancellationToken.None));
        Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
        Assert.AreEqual(exception.Message, "IdToken is invalid.");
    }

    [TestMethod]
    public async Task TokenMiddleware_InvalidAuthorizationHeaderFormat_ThrowsUnauthorized()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<IIdTokenService>();
        _httpContext.Request.Headers.Authorization = "InvalidFormat";

        Guid clientId = Guid.NewGuid();
        var authorization = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:clientSecret"));
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials"",
            ""id_token"": ""validIdToken""
        }";

        var context = DefaultContext.Create([], _mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new AuthorizationService(_mockConfiguration, _mockClientService, mockIdTokenService);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context, CancellationToken.None));
    }
}