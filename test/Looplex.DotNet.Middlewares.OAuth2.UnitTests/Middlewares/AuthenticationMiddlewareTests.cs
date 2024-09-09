using System.Dynamic;
using System.Net;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Middlewares;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.OAuth2.UnitTests.Middlewares;

[TestClass]
public class AuthenticationMiddlewareTests
{
    private IConfiguration _configuration = null!;
    private IJwtService _jwtService = null!;
    private HttpContext _httpContext = null!;
    private IContext _context = null!;
    private Func<Task> _next = null!;

    [TestInitialize]
    public void SetUp()
    {
        // Set up substitutes
        _configuration = Substitute.For<IConfiguration>();
        _jwtService = Substitute.For<IJwtService>();
        _httpContext = new DefaultHttpContext();

        // Set up configuration values
        _configuration["Audience"].Returns("expectedAudience");
        _configuration["Issuer"].Returns("expectedIssuer");
        _configuration["PublicKey"].Returns("dGVzdA==");

        // Set up the middleware context
        _context = Substitute.For<IContext>();
        _context.Services.GetService(typeof(IConfiguration)).Returns(_configuration);
        _context.Services.GetService(typeof(IJwtService)).Returns(_jwtService);
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
        string validToken = "validAccessToken";
        _httpContext.Request.Headers.Authorization = $"Bearer {validToken}";
        _jwtService.ValidateToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);

        // Act
        await AuthenticationMiddleware.AuthenticateMiddleware(_context, CancellationToken.None, _next);

        // Assert
        await _next.Received(1).Invoke();
    }

    [TestMethod]
    public async Task AuthenticateMiddleware_InvalidToken_ThrowsUnauthorizedException()
    {
        // Arrange
        string invalidToken = "invalidAccessToken";
        _httpContext.Request.Headers.Authorization = $"Bearer {invalidToken}";
        _jwtService.ValidateToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(false);

        // Act & Assert
        var ex = await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
            await AuthenticationMiddleware.AuthenticateMiddleware(_context, CancellationToken.None, _next)
        );

        Assert.AreEqual(HttpStatusCode.Unauthorized, ex.StatusCode);
        await _next.DidNotReceive().Invoke();
    }
}