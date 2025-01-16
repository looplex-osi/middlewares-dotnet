using System.Dynamic;
using System.Net;
using FluentAssertions;
using Looplex.DotNet.Core.Application.ExtensionMethods;
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

    private string mockToken =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiZW1haWwiOiJqb2huLmRvZUBlbWFpbC5jb20iLCJpYXQiOjE1MTYyMzkwMjJ9.mock";
    
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
        state.CancellationToken = CancellationToken.None;
        _context.State.Returns(state);

        // Set up the next middleware delegate
        _next = Substitute.For<Func<Task>>();
    }

    [TestMethod]
    public async Task AuthenticateMiddleware_ValidToken_CallsNextMiddleware()
    {
        // Arrange
        _httpContext.Request.Headers.Authorization = $"Bearer {mockToken}";
        _jwtService.ValidateToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);

        // Act
        await OAuth2Middlewares.AuthenticationMiddleware(_context, _next);

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
            await OAuth2Middlewares.AuthenticationMiddleware(_context, _next)
        );

        Assert.AreEqual(HttpStatusCode.Unauthorized, ex.StatusCode);
        await _next.DidNotReceive().Invoke();
    }

    [TestMethod]
    public async Task AuthenticateMiddleware_ValidToken_ContextHasUserInfo()
    {
        // Arrange
        _httpContext.Request.Headers.Authorization = $"Bearer {mockToken}";
        _jwtService.ValidateToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);

        // Act
        await OAuth2Middlewares.AuthenticationMiddleware(_context, _next);

        // Assert
        _context.GetRequiredValue<string>("User.Name").Should().Be("John Doe");
        _context.GetRequiredValue<string>("User.Email").Should().Be("john.doe@email.com");
    }
}