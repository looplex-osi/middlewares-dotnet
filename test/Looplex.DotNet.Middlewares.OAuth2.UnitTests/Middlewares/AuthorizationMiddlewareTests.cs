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
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using static MassTransit.ValidationResultExtensions;

namespace Looplex.DotNet.Middlewares.OAuth2.UnitTests.Middlewares
{
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

            string directory = Directory.GetCurrentDirectory();
            var enforcer = new Enforcer(directory  + "/model.conf", directory + "/policy.csv");
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
            _httpContext.Request.Headers.Authorization = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InJhZmFlbC5pbWFrYXdhQGxvb3BsZXguY29tLmJyIn0.mock-signature";

            _context.State.Returns(state);

            // Set up the next middleware delegate
            _next = Substitute.For<Func<Task>>();
        }

        private MethodInfo GetPrivateMethod(string methodName)
        {
            var method = typeof(AuthorizationMiddleware).GetMethod(
        methodName,
        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
            return method ?? throw new InvalidOperationException($"Method {methodName} not found");
        }

        private string InvokeMethodWithContext(MethodInfo method)
        {
            return (string)(method.Invoke(null, new object[] { _context
})
                ?? throw new InvalidOperationException("Method returned null"));
        }

        public void AuthenticateMiddleware_GetResourceFromURL_LastPathAsID()
        {
            var getResourceFromURL = GetPrivateMethod("GetResourceFromURL");
            HttpContext httpContext = _context.State.HttpContext;
            httpContext.Request.Path = "/cases/24/andamentos/32";

            string result = InvokeMethodWithContext(getResourceFromURL);
            result.Should().Be("andamentos/32");
        }

        [TestMethod]
        public void AuthenticateMiddleware_GetResourceFromURL_LastPathAsResource()
        {

            var getResourceFromURL = GetPrivateMethod("GetResourceFromURL");
            HttpContext httpContext = _context.State.HttpContext;
            httpContext.Request.Path = "/cases/24/andamentos";

            string result = InvokeMethodWithContext(getResourceFromURL);

            result.Should().Be("andamentos");
        }



        [TestMethod]
        public void AuthenticateMiddleware_GetResourceFromURL_SingleElementPath()
        {

            var getResourceFromURL = GetPrivateMethod("GetResourceFromURL");
            HttpContext httpContext = _context.State.HttpContext;
            httpContext.Request.Path = "/cases";

            string result = InvokeMethodWithContext(getResourceFromURL);

            result.Should().Be("cases");
        }

        [TestMethod]
        public void AuthenticateMiddleware_GetResourceFromURL_DoubleElementPath()
        {

            MethodInfo? getResourceFromURL = typeof(AuthorizationMiddleware).GetMethod("GetResourceFromURL", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);

            getResourceFromURL.Should().NotBeNull();
            HttpContext httpContext = _context.State.HttpContext;
            httpContext.Request.Path = "/cases/2";

            string result = (string)getResourceFromURL!.Invoke(null, new object[] { _context })!;

            result.Should().Be("cases/2");
        }


        [TestMethod]
        public void AuthenticateMiddleware_GetUserIdFromToken_SuccessExample()
        {

            MethodInfo? getResourceFromURL = typeof(AuthorizationMiddleware).GetMethod("GetUserIdFromToken", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);

            getResourceFromURL.Should().NotBeNull();
            HttpContext httpContext = _context.State.HttpContext;


            string result = (string)getResourceFromURL!.Invoke(null, new object[] { _context })!;

            result.Should().Be("rafael.imakawa@looplex.com.br");
        }

        private string? InvokeConvertHttpMethodToRbacAction(string httpMethod)
        {
            MethodInfo convertMethod = typeof(AuthorizationMiddleware).GetMethod(
                "ConvertHttpMethodToRbacAction",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new InvalidOperationException("Method not found");

            HttpContext httpContext = _context.State.HttpContext;
            httpContext.Request.Method = httpMethod;

            return (string?)convertMethod.Invoke(null, new object[] { _context });
        }
        [TestMethod]
        public void AuthenticateMiddleware_ConvertHttpMethodToRbacAction_GET()
        {
            var result = InvokeConvertHttpMethodToRbacAction("GET");
            result.Should().Be("read");
        }

        [TestMethod]
        public void AuthenticateMiddleware_ConvertHttpMethodToRbacAction_POST()
        {

            var result = InvokeConvertHttpMethodToRbacAction("POST");
            result.Should().Be("write");
        }

        [TestMethod]
        public void AuthenticateMiddleware_ConvertHttpMethodToRbacAction_PUT()
        {
            var result = InvokeConvertHttpMethodToRbacAction("PUT");
            result.Should().Be("write");
        }
        [TestMethod]
        public void AuthenticateMiddleware_ConvertHttpMethodToRbacAction_DELETE()
        {
            var result = InvokeConvertHttpMethodToRbacAction("DELETE");
            result.Should().Be("delete");
        }

        [TestMethod]
        public void AuthenticateMiddleware_ConvertHttpMethodToRbacAction_Should_Throw_Exception_When_No_Method_Is_Provided()
        {

            MethodInfo getResourceFromURL = typeof(AuthorizationMiddleware).GetMethod("ConvertHttpMethodToRbacAction", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance)!;

            getResourceFromURL.Should().NotBeNull();
            HttpContext httpContext = _context.State.HttpContext;
            httpContext.Request.Method = null!;

            var ex = Assert.ThrowsException<TargetInvocationException>(() =>

            (string?)getResourceFromURL.Invoke(null, new object[] { _context }));

            Assert.AreEqual("Value cannot be null. (Parameter 'HTTP method is null')", ex.InnerException!.Message);
        }


    }
}
