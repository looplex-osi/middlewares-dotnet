using FluentAssertions;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Middlewares;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Dynamic;

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
        public async Task PaginationMiddleware_Should_Throw_Exception_When_Count_Is_Zero()
        {
            // Arrange
            _context.State.HttpContext.Request.QueryString =
                new QueryString("?startIndex=10&count=0");

            // Act & Assert
            var act = () => ScimV2Middlewares.PaginationMiddleware(_context, CancellationToken.None, _next);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Param count is not valid");
        }

        [TestMethod]
        public async Task PaginationMiddleware_Should_Throw_Exception_When_StartIndex_Is_Zero()
        {
            // Arrange
            _context.State.HttpContext.Request.QueryString =
                new QueryString("?startIndex=0&count=10");

            // Act & Assert
            var act = () => ScimV2Middlewares.PaginationMiddleware(_context, CancellationToken.None, _next);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Param startIndex is not valid");
        }

        [TestMethod]
        public async Task PaginationMiddleware_Should_Throw_Exception_When_Count_Is_Negative_Value()
        {
            // Arrange
            _context.State.HttpContext.Request.QueryString =
                new QueryString("?startIndex=10&count=-1");

            // Act & Assert
            var act = () => ScimV2Middlewares.PaginationMiddleware(_context, CancellationToken.None, _next);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Param count is not valid");
        }

        [TestMethod]
        public async Task PaginationMiddleware_Should_Throw_Exception_When_StartIndex_Is_Negative_Value()
        {
            // Arrange
            _context.State.HttpContext.Request.QueryString =
                new QueryString("?startIndex=-5&count=10");

            // Act & Assert
            var act = () => ScimV2Middlewares.PaginationMiddleware(_context, CancellationToken.None, _next);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Param startIndex is not valid");
        }

        [TestMethod]
        public async Task PaginationMiddleware_Should_Throw_Exception_When_Count_Is_Not_Integer()
        {
            // Arrange
            _context.State.HttpContext.Request.QueryString =
                new QueryString("?startIndex=10&count=abc");

            // Act & Assert
            var act = () => ScimV2Middlewares.PaginationMiddleware(_context, CancellationToken.None, _next);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Param count is not valid");
        }

        [TestMethod]
        public async Task PaginationMiddleware_Should_Throw_Exception_When_StartIndex_Is_Not_Integer()
        {
            // Arrange
            _context.State.HttpContext.Request.QueryString =
                new QueryString("?startIndex=-5&count=10.75");

            // Act & Assert
            var act = () => ScimV2Middlewares.PaginationMiddleware(_context, CancellationToken.None, _next);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Param startIndex is not valid");
        }
    }
}