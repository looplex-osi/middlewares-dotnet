using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Dynamic;
using Casbin;
using System.Reflection;
using FluentAssertions;
using Looplex.DotNet.Middlewares.OAuth2.Middlewares;
using Microsoft.Extensions.Logging;

namespace Looplex.DotNet.Middlewares.OAuth2.UnitTests.Middlewares;

[TestClass]
public class AuthorizationMiddlewareTests
{
    private IConfiguration _configuration = null!;
    private IJwtService _jwtService = null!;
    private IRbacService _rbacService = null!;
    private HttpContext _httpContext = null!;
    private IContext _context = null!;
    private Func<Task> _next = null!;
    private ILogger<CasbinRbacService> _logger = null!;

    [TestInitialize]
    public void SetUp()
    {
        // Set up substitutes
        _configuration = Substitute.For<IConfiguration>();
        _jwtService = new JwtService();

        var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                            ?? throw new InvalidOperationException("Could not determine test directory");
        var modelPath = Path.Combine(testDirectory, "model.conf");
        var policyPath = Path.Combine(testDirectory, "policy.csv");

        if (!File.Exists(modelPath) || !File.Exists(policyPath))
        {
            throw new FileNotFoundException("Required Casbin configuration files are missing");
        }

        var enforcer = new Enforcer(modelPath, policyPath);
        _logger = Substitute.For<ILogger<CasbinRbacService>>();
        _rbacService = new CasbinRbacService(enforcer, _logger);
        _httpContext = new DefaultHttpContext();

        // Set up configuration values
        _configuration["Audience"].Returns("expectedAudience");
        _configuration["Issuer"].Returns("expectedIssuer");
        _configuration["PublicKey"].Returns("dGVzdA==");

        // Set up the middleware context
        _context = Substitute.For<IContext>();
        _context.Services.GetService(typeof(IConfiguration)).Returns(_configuration);
        _context.Services.GetService(typeof(IJwtService)).Returns(_jwtService);
        _context.Services.GetService(typeof(IRbacService)).Returns(_rbacService);

        dynamic state = new ExpandoObject();
        state.HttpContext = _httpContext;
        _httpContext.Request.Headers.Authorization =
            "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InJhZmFlbC5pbWFrYXdhQGxvb3BsZXguY29tLmJyIn0.mock-signature";

        _context.State.Returns(state);

        // Set up the next middleware delegate
        _next = Substitute.For<Func<Task>>();
    }

    private MethodInfo GetPrivateMethod(string methodName)
    {
        var method = typeof(OAuth2Middlewares).GetMethod(
            methodName,
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
        return method ?? throw new InvalidOperationException($"Method {methodName} not found");
    }

    private string InvokeMethodWithContext(MethodInfo method)
    {
        return (string)(method.Invoke(null, [
                            _context
                        ])
                        ?? throw new InvalidOperationException("Method returned null"));
    }

    [TestMethod]
    public void AuthenticateMiddleware_GetResourceFromURL_LastPathAsID()
    {
        // Arrange
        var getResourceFromUrl = GetPrivateMethod("GetResourceFromUrl");
        HttpContext httpContext = _context.State.HttpContext;
        httpContext.Request.Path = "/cases/24/andamentos/32";

        // Act
        string result = InvokeMethodWithContext(getResourceFromUrl);
        
        // Assert
        result.Should().Be("andamentos/32");
    }

    [TestMethod]
    public void AuthenticateMiddleware_GetResourceFromURL_LastPathAsResource()
    {
        // Arrange
        var getResourceFromUrl = GetPrivateMethod("GetResourceFromUrl");
        HttpContext httpContext = _context.State.HttpContext;
        httpContext.Request.Path = "/cases/24/andamentos";

        // Act
        string result = InvokeMethodWithContext(getResourceFromUrl);

        // Assert
        result.Should().Be("andamentos");
    }

    [TestMethod]
    public void AuthenticateMiddleware_GetResourceFromURL_SingleElementPath()
    {
        // Arrange
        var getResourceFromUrl = GetPrivateMethod("GetResourceFromUrl");
        HttpContext httpContext = _context.State.HttpContext;
        httpContext.Request.Path = "/cases";

        // Act
        string result = InvokeMethodWithContext(getResourceFromUrl);

        // Assert
        result.Should().Be("cases");
    }

    [TestMethod]
    public void AuthenticateMiddleware_GetResourceFromURL_DoubleElementPath()
    {
        // Arrange
        var getResourceFromUrl = GetPrivateMethod("GetResourceFromUrl");
        getResourceFromUrl.Should().NotBeNull();
        HttpContext httpContext = _context.State.HttpContext;
        httpContext.Request.Path = "/cases/2";

        // Act
        string result = (string)getResourceFromUrl!.Invoke(null, [_context])!;

        // Assert
        result.Should().Be("cases/2");
    }
        
    [TestMethod]
    public void AuthenticateMiddleware_GetUserIdFromToken_SuccessExample()
    {
        // Arrange
        var getResourceFromUrl = GetPrivateMethod("GetUserIdFromToken");
            
        // Act
        string result = (string)getResourceFromUrl!.Invoke(null, [_context])!;

        // Assert
        result.Should().Be("rafael.imakawa@looplex.com.br");
    }

    private string? InvokeConvertHttpMethodToRbacAction(string httpMethod)
    {
        var convertHttpMethodToRbacActionMethod = GetPrivateMethod("ConvertHttpMethodToRbacAction");

        HttpContext httpContext = _context.State.HttpContext;
        httpContext.Request.Method = httpMethod;

        return (string?)convertHttpMethodToRbacActionMethod.Invoke(null, [_context]);
    }

    [TestMethod]
    public void AuthenticateMiddleware_ConvertHttpMethodToRbacAction_GET()
    {
        // Act
        var result = InvokeConvertHttpMethodToRbacAction("GET");
        
        // Assert
        result.Should().Be("read");
    }

    [TestMethod]
    public void AuthenticateMiddleware_ConvertHttpMethodToRbacAction_POST()
    {
        //Act
        var result = InvokeConvertHttpMethodToRbacAction("POST");
        
        // Assert
        result.Should().Be("write");
    }

    [TestMethod]
    public void AuthenticateMiddleware_ConvertHttpMethodToRbacAction_PUT()
    {
        // Act
        var result = InvokeConvertHttpMethodToRbacAction("PUT");
        
        // Assert
        result.Should().Be("write");
    }

    [TestMethod]
    public void AuthenticateMiddleware_ConvertHttpMethodToRbacAction_DELETE()
    {
        // Act
        var result = InvokeConvertHttpMethodToRbacAction("DELETE");
        
        // Assert
        result.Should().Be("delete");
    }

    [TestMethod]
    public void
        AuthenticateMiddleware_ConvertHttpMethodToRbacAction_Should_Throw_Exception_When_No_Method_Is_Provided()
    {
        // Arrange
        var convertHttpMethodToRbacActionMethod = GetPrivateMethod("ConvertHttpMethodToRbacAction");
        HttpContext httpContext = _context.State.HttpContext;
        httpContext.Request.Method = null!;

        // Act & Assert
        var ex = Assert.ThrowsException<TargetInvocationException>(() =>
            (string?)convertHttpMethodToRbacActionMethod.Invoke(null, [_context]));

        // Assert
        Assert.AreEqual("HTTP method is null (Parameter 'context')", ex.InnerException!.Message);
    }
}