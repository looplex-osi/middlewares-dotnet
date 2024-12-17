using Castle.Core.Logging;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Looplex.DotNet.Middlewares.OAuth2.Middlewares;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Casbin;

namespace Looplex.DotNet.Middlewares.OAuth2.UnitTests.Middlewares
{
    [TestClass]
    public class AuthorizationMiddlewareTests
    {
        private IConfiguration _configuration = null!;
        private IJwtService _jwtService = null!;
        private IDomainProviderService _domainProviderService = null;
        private IRBACService _rbacService = null;
        private HttpContext _httpContext = null!;
        private IContext _context = null!;
        private Func<Task> _next = null!;

        [TestInitialize]
        public void SetUp()
        {
            // Set up substitutes
            _configuration = Substitute.For<IConfiguration>();
            _jwtService = new JwtService();
            _domainProviderService = new DomainProviderService();

            string directory = Directory.GetCurrentDirectory();
            var enforcer = new Enforcer(directory + "/model.conf", directory +"/policy.csv");
            _rbacService = new CasbinRBACService(enforcer);
            _httpContext = new DefaultHttpContext();

            // Set up configuration values
            _configuration["Audience"].Returns("expectedAudience");
            _configuration["Issuer"].Returns("expectedIssuer");
            _configuration["PublicKey"].Returns("dGVzdA==");

            // Set up the middleware context
            _context = Substitute.For<IContext>();
            _context.Services.GetService(typeof(IConfiguration)).Returns(_configuration);
            _context.Services.GetService(typeof(IJwtService)).Returns(_jwtService);
            _context.Services.GetService(typeof(IDomainProviderService)).Returns(_domainProviderService);
            _context.Services.GetService(typeof(IRBACService)).Returns(_rbacService);

            dynamic state = new ExpandoObject();
            state.HttpContext = _httpContext;
            
            _context.State.Returns(state);

            // Set up the next middleware delegate
            _next = Substitute.For<Func<Task>>();
        }

        [TestMethod]
        public async Task AuthenticateMiddleware_ValidToken_CallsNextMiddleware()
        {
            // Arrange
            string validToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IlJhZmFlbCBSYWZhZWwiLCJlbWFpbCI6InJhZmFlbC5pbWFrYXdhQGxvb3BsZXguY29tLmJyIiwiUGhvdG8iOiJodHRwczovL2dyYXBoLm1pY3Jvc29mdC5jb20vdjEuMC9tZS9waG90by8kdmFsdWUiLCJuYmYiOjE3MzM5NDIyMjAsImV4cCI6MTczMzk0NTgyMCwiaWF0IjoxNzMzOTQyMjIwLCJpc3MiOiJkb3RuZXQubG9vcGxleC5jb20uYnIiLCJhdWQiOiJzYWFzLmxvb3BsZXguY29tLmJyIn0.NuDSJbMUVhqdZGK8vb-rAn0ETv-Vi3raakQdB-yGZBLdEQ3luzdw8zytD6VGhM2-JI3VLZ_I0ZkUBUurOXGzbwXy6fMAMgLZOz4WDJ6Boa3LlbJkSqyQumu9ACHu3bEZMSwvLuqWgL1_AT_9NcJsxPpj63KNqmhZwx30hYsTm1SsYNU49XeeBPDzGeaWk6N44qnrcNjPapLhbGLBi6iaGJOyniGCMjCZFd8Pwv3JF3peZ_xW7omw6XA5CTpzbuNg50Eo3D0ZCwwJoC4Kid-LvZ0TWjARrCChm3Wdv81i6bXDxKgjIcqtkCvz4G1wzQKb9hE3HiaQEx_3tPPbxj68_nQqMh_ZP-pDbrHJRzkZy5jBKw1SutrSBuA3_HDATYPzF9feE-DvrrZF42aZIFTYZ7ndN1qJxfdOb4mSk3J1r9qTuCS7h5t6UF0qTOkShxU01Ve3F4YmfVsop0qsaGJicEh0vf142pWmLFjy4IxNHb-Nm_rk6oImckLO0qyI6-lVMI-C1x5ImsvMX6gT0nJqOF872ewbSykmS2PGkatOBHP7DwI_T612kKMCAoVoB7D46Me1AxvDdNi_4pGnTdoEabNAF9OdrG6CXGQtCilwRcNV8bWGdprcMhgfXpq5EnStRYva4SVPida8fEQDzfFL-I560G1tV1owYKgxY4dXO2k";
            _httpContext.Request.Headers.Authorization = $"Bearer {validToken}";
            _httpContext.Request.Headers.Add("X-looplex-tenant", "TestDomain");
            _httpContext.Request.Path = "/cases/24/andamentos/32";
            _httpContext.Request.Method = "POST";

            // Act
            await AuthorizationMiddleware.AuthorizeMiddleware(_context, CancellationToken.None, _next);

            // Assert
            await _next.Received(1).Invoke();
        }

        
    }
}
