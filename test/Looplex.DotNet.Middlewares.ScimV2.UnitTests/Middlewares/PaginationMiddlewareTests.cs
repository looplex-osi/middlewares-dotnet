using FluentAssertions;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Middlewares;
using Looplex.DotNet.Middlewares.ScimV2.Providers;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Middlewares
{
    [TestClass]
    public class PaginationMiddlewareTests
    {
        private IScimV2Context _context = null!;
        private Func<Task> _next = null!;

        [TestInitialize]
        public void Setup()
        {
            // Mock dependencies
            _context = Substitute.For<IScimV2Context>();
            dynamic state = new ExpandoObject();
            state.HttpContext = new DefaultHttpContext();
            _context.State.Returns(state);

            // Set up the next middleware delegate
            _next = Substitute.For<Func<Task>>();

        }

        [TestMethod]
        public  async Task PaginationMiddleware_Should_Throw_Exception()
        {

            // Arrange for invalid count

            HttpContext httpContext = _context.State.HttpContext;

            httpContext.Request.QueryString = new QueryString("?startIndex=1&count=0");

            // Act & Assert

            var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                 await ScimV2Middlewares.PaginationMiddleware(_context, CancellationToken.None, _next)
            );

            Assert.AreEqual("Param count is not valid", ex.Message);

            // Arrange for invalid startIndex

            httpContext.Request.QueryString = new QueryString("?startIndex=0&count=10");

            // Act & Assert

            ex = await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                 await ScimV2Middlewares.PaginationMiddleware(_context, CancellationToken.None, _next)
            );

            Assert.AreEqual("Param startIndex is not valid", ex.Message);
        }
    }
}
