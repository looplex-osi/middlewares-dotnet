using System.Net;
using FluentAssertions;
using Looplex.DotNet.Core.Application.Abstractions.Providers;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.UnitTests;

[TestClass]
public class DefaultScimV2ContextTests
{
    private IServiceProvider _services = null!;
    private ISqlDatabaseProvider _sqlDatabaseProvider = null!;
    private DefaultScimV2Context _scimContext = null!;

    [TestInitialize]
    public void Setup()
    {
        _services = Substitute.For<IServiceProvider>();
        _sqlDatabaseProvider = Substitute.For<ISqlDatabaseProvider>();
        _scimContext = new DefaultScimV2Context(_services, _sqlDatabaseProvider);
    }

    [TestMethod]
    public async Task GetSqlDatabaseService_ShouldReturnDatabaseService_WhenHeaderContainsValidTenantKey()
    {
        // Arrange
        var domain = "example.com";
        var mockDatabaseService = Substitute.For<ISqlDatabaseService>();

        _scimContext.Headers["X-looplex-tenant"] = domain;
        _sqlDatabaseProvider.GetDatabaseAsync(domain).Returns(Task.FromResult(mockDatabaseService));

        // Act
        var result = await _scimContext.GetSqlDatabaseService();

        // Assert
        result.Should().Be(mockDatabaseService);
        await _sqlDatabaseProvider.Received(1).GetDatabaseAsync(domain);
    }

    [TestMethod]
    public async Task GetSqlDatabaseService_ShouldThrowError_WhenHeaderDoesNotContainTenantKey()
    {
        // Act
        Func<Task> act = async () => await _scimContext.GetSqlDatabaseService();

        // Assert
        await act.Should().ThrowAsync<Error>()
            .WithMessage($"X-looplex-tenant not found in context header.")
            .Where(e => e.Status == (int)HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task GetSqlDatabaseService_ShouldThrowError_WhenDomainIsNullOrWhitespace()
    {
        // Arrange
        _scimContext.Headers["X-looplex-tenant"] = " ";

        // Act
        Func<Task> act = async () => await _scimContext.GetSqlDatabaseService();

        // Assert
        await act.Should().ThrowAsync<Error>()
            .WithMessage("Domain should not be null or empty.")
            .Where(e => e.Status == (int)HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task GetSqlDatabaseService_ShouldNotCallDatabaseProvider_WhenAlreadyInitialized()
    {
        // Arrange
        var mockDatabaseService = Substitute.For<ISqlDatabaseService>();
        _scimContext.Headers["X-looplex-tenant"] = "example.com";

        // Set the private `_sqlDatabaseService` field
        typeof(DefaultScimV2Context)
            .GetField("_sqlDatabaseService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(_scimContext, mockDatabaseService);

        // Act
        var result = await _scimContext.GetSqlDatabaseService();

        // Assert
        result.Should().Be(mockDatabaseService);
        await _sqlDatabaseProvider.DidNotReceive().GetDatabaseAsync(Arg.Any<string>());
    }
}