using System.Dynamic;
using System.Net;
using System.Text;
using FluentAssertions;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Middlewares.ApiKeys.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Dtos;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.UnitTests.Services;

[TestClass]
public class ClientCredentialsAuthorizationServiceTests
{
    private IConfiguration _mockConfiguration = null!;
    private IApiKeyService _mockApiKeyService = null!;
    private IServiceProvider _mockServiceProvider = null!;
    private IContextFactory _mockContextFactory = null!;
    private IJwtService _mockJwtService = null!;
    private DefaultHttpContext _httpContext = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockConfiguration = Substitute.For<IConfiguration>();
        _mockApiKeyService = Substitute.For<IApiKeyService>();
        _mockServiceProvider = Substitute.For<IServiceProvider>();
        _mockContextFactory = Substitute.For<IContextFactory>();
        _mockJwtService = Substitute.For<IJwtService>();
        _httpContext = new DefaultHttpContext();
        
        var configurationSection = Substitute.For<IConfigurationSection>();
        configurationSection.Value.Returns("20");
        _mockConfiguration.GetSection("TokenExpirationTimeInMinutes").Returns(configurationSection);

        _mockServiceProvider.GetService(typeof(IContextFactory)).Returns(_mockContextFactory);
    }

    [TestMethod]
    public async Task CreateAccessToken_InvalidAuth_ThrowsUnauthorized()
    {
        // Arrange
        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["OicdTenantId"].Returns("oicdTenantId");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));

        var authorization = "Bearer xxxxxx";
        var clientCredentials = @"{
            ""grant_type"": ""urn:ietf:params:oauth:grant-type:token-exchange""
        }";
        
        var context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        context.State.Returns(state);
        context.Services.Returns(_mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new ClientCredentialsAuthorizationService(_mockConfiguration, _mockApiKeyService, _mockJwtService);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context, CancellationToken.None));
        Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
        Assert.AreEqual(exception.Message, "Invalid authorization.");
    }

    [TestMethod]
    public async Task CreateAccessToken_InvalidGrantType_ThrowsUnauthorized()
    {
        // Arrange
        var authorization = "Basic xxxxxx";
        var clientCredentials = @"{
            ""grant_type"": ""invalid""
        }";
        
        var context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        context.State.Returns(state);
        context.Services.Returns(_mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new ClientCredentialsAuthorizationService(_mockConfiguration, _mockApiKeyService, _mockJwtService);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context, CancellationToken.None));
        Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
        Assert.AreEqual(exception.Message, "grant_type is invalid.");
    }

    [TestMethod]
    public async Task CreateAccessToken_ValidBasicAuth_ReturnsAccessToken()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var clientSecret = "clientSecret";
        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["OicdTenantId"].Returns("oicdTenantId");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));
        
        var authorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:clientSecret"));
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials""
        }";
        
        var contextChild = Substitute.For<IContext>();
        var stateChild = new ExpandoObject();
        contextChild.State.Returns(stateChild);
        var roles = new Dictionary<string, dynamic>();
        contextChild.Roles.Returns(roles);
        _mockContextFactory.Create(Arg.Any<IEnumerable<string>>()).Returns(contextChild);
        
        var apiKey = Substitute.For<IApiKey>();
        apiKey.ClientId.Returns(clientId);
        apiKey.ClientName.Returns("client");
        apiKey.Digest.Returns("digest");
        apiKey.NotBefore.Returns(DateTimeOffset.UtcNow.AddMinutes(-1));
        apiKey.ExpirationTime.Returns(DateTimeOffset.UtcNow.AddMinutes(1));

        _mockApiKeyService.GetByIdAndSecretOrDefaultAsync(
                Arg.Is<IContext>(c => AssertDefaultContextIsValid(c, clientId, clientSecret)))
            .Returns(call =>
            {
                var context = call.Arg<IContext>();
                context.Roles["ApiKey"] = apiKey;
                return Task.CompletedTask;
            });

        var context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        context.State.Returns(state);
        context.Services.Returns(_mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new ClientCredentialsAuthorizationService(_mockConfiguration, _mockApiKeyService, _mockJwtService);

        // Act
        await service.CreateAccessToken(context, CancellationToken.None);

        // Assert
        Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
        Assert.IsNotNull(context.Result);
        Assert.IsInstanceOfType(context.Result, typeof(AccessTokenDto));
    }

    private bool AssertDefaultContextIsValid(IContext c, Guid clientId, string clientSecret)
    {
        return c.State.ClientId == clientId.ToString()
               && c.State.ClientSecret == clientSecret;
    }

    [TestMethod]
    public async Task CreateAccessToken_ApiKeyNotFound_ThrowsEntityInvalidException()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var clientSecret = "clientSecret";

        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["OicdTenantId"].Returns("oicdTenantId");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));

        var authorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:clientSecret"));
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials""
        }";

        var contextChild = Substitute.For<IContext>();
        var stateChild = new ExpandoObject();
        contextChild.State.Returns(stateChild);
        var roles = new Dictionary<string, dynamic>();
        contextChild.Roles.Returns(roles);
        _mockContextFactory.Create(Arg.Any<IEnumerable<string>>()).Returns(contextChild);
        
        var apiKey = Substitute.For<IApiKey>();
        apiKey.ClientId.Returns(clientId);
        apiKey.ClientName.Returns("client");
        apiKey.Digest.Returns("digest");
        apiKey.NotBefore.Returns(DateTimeOffset.UtcNow.AddMinutes(1));
        apiKey.ExpirationTime.Returns(DateTimeOffset.UtcNow.AddMinutes(1));

        _mockApiKeyService.GetByIdAndSecretOrDefaultAsync(
                Arg.Is<IContext>(c => AssertDefaultContextIsValid(c, clientId, clientSecret)))
            .Returns(_ => Task.CompletedTask);

        var context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        context.State.Returns(state);
        context.Services.Returns(_mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new ClientCredentialsAuthorizationService(_mockConfiguration, _mockApiKeyService, _mockJwtService);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<EntityInvalidException>(() => service.CreateAccessToken(context, CancellationToken.None));
        exception.ErrorMessages.Should().Contain("Invalid clientId or clientSecret.");
    }

    [TestMethod]
    public async Task CreateAccessToken_ApiKeyNotBeforeError_ThrowsEntityInvalidException()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var clientSecret = "clientSecret";

        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["OicdTenantId"].Returns("oicdTenantId");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));

        var authorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:clientSecret"));
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials""
        }";

        var contextChild = Substitute.For<IContext>();
        var stateChild = new ExpandoObject();
        contextChild.State.Returns(stateChild);
        var roles = new Dictionary<string, dynamic>();
        contextChild.Roles.Returns(roles);
        _mockContextFactory.Create(Arg.Any<IEnumerable<string>>()).Returns(contextChild);
        
        var apiKey = Substitute.For<IApiKey>();
        apiKey.ClientId.Returns(clientId);
        apiKey.ClientName.Returns("client");
        apiKey.Digest.Returns("digest");
        apiKey.NotBefore.Returns(DateTimeOffset.UtcNow.AddMinutes(1));
        apiKey.ExpirationTime.Returns(DateTimeOffset.UtcNow.AddMinutes(1));

        _mockApiKeyService.GetByIdAndSecretOrDefaultAsync(
                Arg.Is<IContext>(c => AssertDefaultContextIsValid(c, clientId, clientSecret)))
            .Returns(call =>
            {
                var context = call.Arg<IContext>();
                context.Roles["ApiKey"] = apiKey;
                return Task.CompletedTask;
            });

        var context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        context.State.Returns(state);
        context.Services.Returns(_mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new ClientCredentialsAuthorizationService(_mockConfiguration, _mockApiKeyService, _mockJwtService);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<EntityInvalidException>(() => service.CreateAccessToken(context, CancellationToken.None));
        exception.ErrorMessages.Should().Contain("Client access not allowed.");
    }
    
    [TestMethod]
    public async Task CreateAccessToken_ApiKeyExpiredError_ThrowsEntityInvalidException()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var clientSecret = "clientSecret";

        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["OicdTenantId"].Returns("oicdTenantId");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));

        var authorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:clientSecret"));
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials""
        }";

        var contextChild = Substitute.For<IContext>();
        var stateChild = new ExpandoObject();
        contextChild.State.Returns(stateChild);
        var roles = new Dictionary<string, dynamic>();
        contextChild.Roles.Returns(roles);
        _mockContextFactory.Create(Arg.Any<IEnumerable<string>>()).Returns(contextChild);
        
        var apiKey = Substitute.For<IApiKey>();
        apiKey.ClientId.Returns(clientId);
        apiKey.ClientName.Returns("client");
        apiKey.Digest.Returns("digest");
        apiKey.NotBefore.Returns(DateTimeOffset.UtcNow.AddMinutes(-1));
        apiKey.ExpirationTime.Returns(DateTimeOffset.UtcNow.AddMinutes(-1));

        _mockApiKeyService.GetByIdAndSecretOrDefaultAsync(
                Arg.Is<IContext>(c => AssertDefaultContextIsValid(c, clientId, clientSecret)))
            .Returns(call =>
            {
                var context = call.Arg<IContext>();
                context.Roles["ApiKey"] = apiKey;
                return Task.CompletedTask;
            });

        var context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        context.State.Returns(state);
        context.Services.Returns(_mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new ClientCredentialsAuthorizationService(_mockConfiguration, _mockApiKeyService, _mockJwtService);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<EntityInvalidException>(() => service.CreateAccessToken(context, CancellationToken.None));
        exception.ErrorMessages.Should().Contain("Client access is expired.");
    }

    [TestMethod]
    public async Task CreateAccessToken_InvalidAuthorizationHeaderFormat_ThrowsUnauthorized()
    {
        // Arrange
        var authorization = "Basic InvalidFormat";
        var clientCredentials = @"{
            ""grant_type"": ""client_credentials"",
            ""subject_token"": ""validToken""
        }";

        var context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        context.State.Returns(state);
        context.Services.Returns(_mockServiceProvider);
        context.State.Authorization = authorization;
        context.State.Resource = clientCredentials;
        var service = new ClientCredentialsAuthorizationService(_mockConfiguration, _mockApiKeyService, _mockJwtService);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<FormatException>(() => service.CreateAccessToken(context, CancellationToken.None));
    }
}