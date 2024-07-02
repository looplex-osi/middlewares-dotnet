using Looplex.DotNet.Middlewares.OAuth2.DTOs;
using Looplex.DotNet.Middlewares.OAuth2.Entities;
using Looplex.DotNet.Middlewares.OAuth2.Services;
using Looplex.OpenForExtension.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Looplex.DotNet.Middlewares.OAuth2.UnitTests.Services
{
    [TestClass]
    public class AuthorizationServiceTests
    {
        private IConfiguration _mockConfiguration = null!;
        private IClientCredentialService _mockClientCredentialService = null!;
        private IServiceProvider _mockServiceProvider = null!;
        private DefaultHttpContext _httpContext = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockConfiguration = Substitute.For<IConfiguration>();
            _mockClientCredentialService = Substitute.For<IClientCredentialService>();
            _mockServiceProvider = Substitute.For<IServiceProvider>();
            _httpContext = new DefaultHttpContext();

            var configurationSection = Substitute.For<IConfigurationSection>();
            configurationSection.Value.Returns("20");
            _mockConfiguration.GetSection("TokenExpirationTimeInMinutes").Returns(configurationSection);
        }

        [TestMethod]
        public async Task TokenMiddleware_InvalidAuth_ThrowsUnauthorized()
        {
            // Arrange
            var _mockIdTokenService = Substitute.For<IIdTokenService>();
            var authorization = "Basic xxxxxx";
            var clientCredentials = new ClientCredentialsDTO
            {
                GrantType = "client_credentials",
                IdToken = "validToken"
            };
            var context = DefaultContext.Create([], _mockServiceProvider);
            context.State.Authorization = authorization;
            context.State.ClientCredentialsDTO = clientCredentials;
            var service = new AuthorizationService(_mockConfiguration, _mockClientCredentialService, _mockIdTokenService);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context));
            Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
            Assert.AreEqual(exception.Message, "Invalid authorization.");
        }

        [TestMethod]
        public async Task TokenMiddleware_InvalidGrantType_ThrowsUnauthorized()
        {
            // Arrange
            var _mockIdTokenService = Substitute.For<IIdTokenService>();
            var authorization = "Bearer xxxxxx";
            var clientCredentials = new ClientCredentialsDTO
            {
                GrantType = "invalid",
                IdToken = "validToken"
            };
            var context = DefaultContext.Create([], _mockServiceProvider);
            context.State.Authorization = authorization;
            context.State.ClientCredentialsDTO = clientCredentials;
            var service = new AuthorizationService(_mockConfiguration, _mockClientCredentialService, _mockIdTokenService);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context));
            Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
            Assert.AreEqual(exception.Message, "grant_type is invalid.");
        }

        [TestMethod]
        public async Task TokenMiddleware_ValidBearerAdminAuth_ReturnsAccessToken()
        {
            // Arrange
            var _mockIdTokenService = Substitute.For<IIdTokenService>();
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
            var clientCredentials = new ClientCredentialsDTO
            {
                GrantType = "client_credentials",
                IdToken = "validIdToken"
            };

            _mockIdTokenService.ValidateIdToken("oicdIssuer", "oicdTenantId", "oicdAudience", "validIdToken", out Arg.Any<string?>()).Returns(x =>
            {
                x[4] = email;
                return true;
            });

            var context = DefaultContext.Create([], _mockServiceProvider);
            context.State.Authorization = authorization;
            context.State.ClientCredentialsDTO = clientCredentials;
            var service = new AuthorizationService(_mockConfiguration, _mockClientCredentialService, _mockIdTokenService);

            // Act
            await service.CreateAccessToken(context);

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
            Assert.IsNotNull(context.Result);
            Assert.IsInstanceOfType(context.Result, typeof(string));
        }

        [TestMethod]
        public async Task TokenMiddleware_InvalidBearerAdminAuthClientSecretIsWrong_ThrowsUnauthorized()
        {
            // Arrange
            var _mockIdTokenService = Substitute.For<IIdTokenService>();
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
            var clientCredentials = new ClientCredentialsDTO
            {
                GrantType = "client_credentials",
                IdToken = "validIdToken"
            };

            _mockIdTokenService.ValidateIdToken("oicdIssuer", "oicdTenantId", "oicdAudience", "validIdToken", out Arg.Any<string?>()).Returns(x =>
            {
                x[4] = email;
                return true;
            });

            _mockClientCredentialService.GetByIdAndSecretOrDefaultAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(default(Client?));

            var context = DefaultContext.Create([], _mockServiceProvider);
            context.State.Authorization = authorization;
            context.State.ClientCredentialsDTO = clientCredentials;
            var service = new AuthorizationService(_mockConfiguration, _mockClientCredentialService, _mockIdTokenService);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context));
            Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
            Assert.AreEqual(exception.Message, "Invalid clientId or clientSecret.");
        }

        [TestMethod]
        public async Task TokenMiddleware_InvalidBearerAdminAuthClientIdIsWrong_ThrowsUnauthorized()
        {
            // Arrange
            var _mockIdTokenService = Substitute.For<IIdTokenService>();
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
            var clientCredentials = new ClientCredentialsDTO
            {
                GrantType = "client_credentials",
                IdToken = "validIdToken"
            };

            _mockIdTokenService.ValidateIdToken("oicdIssuer", "oicdTenantId", "oicdAudience", "validIdToken", out Arg.Any<string?>()).Returns(x =>
            {
                x[4] = email;
                return true;
            });

            _mockClientCredentialService.GetByIdAndSecretOrDefaultAsync(Arg.Any<Guid>(), Arg.Any<string>()).Returns(default(Client?));

            var context = DefaultContext.Create([], _mockServiceProvider);
            context.State.Authorization = authorization;
            context.State.ClientCredentialsDTO = clientCredentials;
            var service = new AuthorizationService(_mockConfiguration, _mockClientCredentialService, _mockIdTokenService);
            
            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context));
            Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
            Assert.AreEqual(exception.Message, "Invalid clientId or clientSecret.");
        }

        [TestMethod]
        public async Task TokenMiddleware_ValidBearerAuth_ReturnsAccessToken()
        {
            // Arrange
            var _mockIdTokenService = Substitute.For<IIdTokenService>();
            var email = "test@example.com";
            var clientId = Guid.NewGuid();
            var clientId1 = Guid.NewGuid();
            var clientSecret = "clientSecret";
            var clientCredentialsDTO = new ClientCredentialsDTO
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
            var clientCredentials = new ClientCredentialsDTO
            {
                GrantType = "client_credentials",
                IdToken = "validIdToken"
            };

            _mockIdTokenService.ValidateIdToken("oicdIssuer", "oicdTenantId", "oicdAudience", "validIdToken", out Arg.Any<string?>()).Returns(x =>
            {
                x[4] = email;
                return true;
            });

            _mockClientCredentialService.GetByIdAndSecretOrDefaultAsync(clientId, clientSecret).Returns(new Client
            {
                ClientId = clientId.ToString(),
                DisplayName = "client",
                Secret = clientSecret,
                NotBefore = DateTime.UtcNow.AddMinutes(-1),
                ExpirationTime = DateTime.UtcNow.AddMinutes(1)
            });

            _httpContext.Request.Headers.Authorization = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret));
            _httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clientCredentialsDTO)));

            var context = DefaultContext.Create([], _mockServiceProvider);
            context.State.Authorization = authorization;
            context.State.ClientCredentialsDTO = clientCredentials;
            var service = new AuthorizationService(_mockConfiguration, _mockClientCredentialService, _mockIdTokenService);

            // Act
            await service.CreateAccessToken(context);

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, _httpContext.Response.StatusCode);
            Assert.IsNotNull(context.Result);
            Assert.IsInstanceOfType(context.Result, typeof(string));
        }

        [TestMethod]
        public async Task TokenMiddleware_InvalidBearerAuth_ThrowsUnauthorized()
        {
            // Arrange
            var _mockIdTokenService = Substitute.For<IIdTokenService>();
            var clientId = Guid.NewGuid();
            var clientSecret = "clientSecret";
            var clientCredentialsDTO = new ClientCredentialsDTO
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
            var clientCredentials = new ClientCredentialsDTO
            {
                GrantType = "client_credentials",
                IdToken = "validIdToken"
            };

            _mockIdTokenService.ValidateIdToken("oicdIssuer", "oicdTenantId", "oicdAudience", "validIdToken", out Arg.Any<string?>()).Returns(x =>
            {
                x[4] = null;
                return false;
            });

            _mockClientCredentialService.GetByIdAndSecretOrDefaultAsync(clientId, clientSecret).Returns(new Client
            {
                ClientId = clientId.ToString(),
                DisplayName = "client",
                Secret = clientSecret,
                NotBefore = DateTime.UtcNow.AddMinutes(-1),
                ExpirationTime = DateTime.UtcNow.AddMinutes(1)
            });

            _httpContext.Request.Headers.Authorization = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret));
            _httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clientCredentialsDTO)));

            var context = DefaultContext.Create([], _mockServiceProvider);
            context.State.Authorization = authorization;
            context.State.ClientCredentialsDTO = clientCredentials;
            var service = new AuthorizationService(_mockConfiguration, _mockClientCredentialService, _mockIdTokenService);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context));
            Assert.AreEqual(exception.StatusCode, HttpStatusCode.Unauthorized);
            Assert.AreEqual(exception.Message, "IdToken is invalid.");
        }

        [TestMethod]
        public async Task TokenMiddleware_InvalidAuthorizationHeaderFormat_ThrowsUnauthorized()
        {
            // Arrange
            var _mockIdTokenService = Substitute.For<IIdTokenService>();
            _httpContext.Request.Headers.Authorization = "InvalidFormat";

            Guid clientId = Guid.NewGuid();
            var authorization = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:clientSecret"));
            var clientCredentials = new ClientCredentialsDTO
            {
                GrantType = "client_credentials",
                IdToken = "validIdToken"
            };

            var context = DefaultContext.Create([], _mockServiceProvider);
            context.State.Authorization = authorization;
            context.State.ClientCredentialsDTO = clientCredentials;
            var service = new AuthorizationService(_mockConfiguration, _mockClientCredentialService, _mockIdTokenService);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => service.CreateAccessToken(context));
        }
    }
}