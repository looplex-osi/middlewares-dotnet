using System.Dynamic;
using System.Net;
using System.Text;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Dtos;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.UnitTests.Services;

[TestClass]
public class TokenExchangeAuthorizationServiceTests
{
    private IConfiguration _mockConfiguration = null!;
    private IServiceProvider _mockServiceProvider = null!;
    private IContextFactory _mockContextFactory = null!;
    private IJwtService _mockJwtService = null!;
    private DefaultHttpContext _httpContext = null!;
    private IHttpClientFactory _httpClientFactory = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockConfiguration = Substitute.For<IConfiguration>();
        _mockServiceProvider = Substitute.For<IServiceProvider>();
        _mockContextFactory = Substitute.For<IContextFactory>();
        _mockJwtService = Substitute.For<IJwtService>();
        _httpContext = new DefaultHttpContext();
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        
        var handlerMock = new HttpMessageHandlerMock();
        var httpClient = new HttpClient(handlerMock);
        _httpClientFactory.CreateClient().Returns(httpClient);
        
        var configurationSection = Substitute.For<IConfigurationSection>();
        configurationSection.Value.Returns("20");
        _mockConfiguration.GetSection("TokenExpirationTimeInMinutes").Returns(configurationSection);

        _mockServiceProvider.GetService(typeof(IContextFactory)).Returns(_mockContextFactory);
    }
    [TestMethod]
    public async Task CreateAccessToken_InvalidGrantType_ThrowsUnauthorized()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<ITokenService>();
        var clientCredentials = @"{
            ""grant_type"": ""invalid"",
            ""subject_token"": ""invalid"",
            ""subject_token_type"": ""invalid""
        }";
        
        var context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        context.State.Returns(state);
        context.Services.Returns(_mockServiceProvider);
        context.State.Resource = clientCredentials;
        var service = new TokenExchangeAuthorizationService(_mockConfiguration, mockIdTokenService, _mockJwtService, _httpClientFactory);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context, CancellationToken.None));
        Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
        Assert.AreEqual(exception.Message, "grant_type is invalid.");
    }

    [TestMethod]
    public async Task CreateAccessToken_InvalidSubjectTokenType_ThrowsUnauthorized()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<ITokenService>();
        var clientCredentials = @"{
            ""grant_type"": ""urn:ietf:params:oauth:grant-type:token-exchange"",
            ""subject_token"": ""invalid"",
            ""subject_token_type"": ""invalid""
        }";
        
        var context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        context.State.Returns(state);
        context.Services.Returns(_mockServiceProvider);
        context.State.Resource = clientCredentials;
        var service = new TokenExchangeAuthorizationService(_mockConfiguration, mockIdTokenService, _mockJwtService, _httpClientFactory);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context, CancellationToken.None));
        Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
        Assert.AreEqual("subject_token_type is invalid.", exception.Message);
    }

    [TestMethod]
    public async Task CreateAccessToken_ValidToken_ReturnsAccessToken()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<ITokenService>();
        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));
        _mockConfiguration["OicdUserInfoEndpoint"].Returns("https://graph.microsoft.com/oidc/userinfo");

        var clientCredentials = @"{
            ""grant_type"": ""urn:ietf:params:oauth:grant-type:token-exchange"",
            ""subject_token"": ""validToken"",
            ""subject_token_type"": ""urn:ietf:params:oauth:token-type:access_token""
        }";

        mockIdTokenService.ValidateToken("oicdIssuer", "oicdAudience", "validToken").Returns(true);
        
        var context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        context.State.Returns(state);
        context.Services.Returns(_mockServiceProvider);
        context.State.Resource = clientCredentials;
        var service = new TokenExchangeAuthorizationService(_mockConfiguration, mockIdTokenService, _mockJwtService, _httpClientFactory);

        // Act
        await service.CreateAccessToken(context, CancellationToken.None);

        // Assert
        Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
        Assert.IsNotNull(context.Result);
        Assert.IsInstanceOfType(context.Result, typeof(AccessTokenDto));
    }

    [TestMethod]
    public async Task CreateAccessToken_InvalidToken_ReturnsAccessToken()
    {
        // Arrange
        var mockIdTokenService = Substitute.For<ITokenService>();
        _mockConfiguration["Audience"].Returns("audience");
        _mockConfiguration["Issuer"].Returns("issuer");
        _mockConfiguration["OicdAudience"].Returns("oicdAudience");
        _mockConfiguration["OicdIssuer"].Returns("oicdIssuer");
        _mockConfiguration["PublicKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PublicKey)));
        _mockConfiguration["PrivateKey"].Returns(Convert.ToBase64String(Encoding.UTF8.GetBytes(RsaKeys.PrivateKey)));
        _mockConfiguration["OicdUserInfoEndpoint"].Returns("https://graph.microsoft.com/oidc/userinfo");

        var clientCredentials = @"{
            ""grant_type"": ""urn:ietf:params:oauth:grant-type:token-exchange"",
            ""subject_token"": ""invalid"",
            ""subject_token_type"": ""urn:ietf:params:oauth:token-type:access_token""
        }";

        mockIdTokenService.ValidateToken("oicdIssuer", "oicdAudience", "validToken").Returns(false);
        
        var context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        context.State.Returns(state);
        context.Services.Returns(_mockServiceProvider);
        context.State.Resource = clientCredentials;
        var service = new TokenExchangeAuthorizationService(_mockConfiguration, mockIdTokenService, _mockJwtService, _httpClientFactory);

        
        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context, CancellationToken.None));
        Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
        Assert.AreEqual(exception.Message, "Token is invalid.");
    }
    
    private class HttpMessageHandlerMock : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Ensure the Authorization header contains the Bearer token
            Assert.AreEqual("validToken", request.Headers.Authorization!.Parameter);

            var userInfo = new UserInfo
            {
                Sub = Guid.NewGuid().ToString(),
                Email = "foo@bar",
                FamilyName = "Bar",
                GivenName = "Foo",
                Name = "Bar",
                Picture = "fb"
            };
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(userInfo), Encoding.UTF8, "application/json")
            });
        }
    }
}