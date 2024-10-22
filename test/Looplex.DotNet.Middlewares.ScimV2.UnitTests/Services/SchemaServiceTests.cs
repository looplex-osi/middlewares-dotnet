using System.Dynamic;
using FluentAssertions;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Services;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using RestSharp;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Services;

[TestClass]
public class SchemaServiceTests
{
    private SchemaService _schemaService = null!;
    private IConfiguration _configuration = null!;
    private ICacheService _cacheService = null!;
    private IRestClient _restClient = null!;
    private IContext _context = null!;

    [TestInitialize]
    public void Setup()
    {
        // Mock dependencies
        _configuration = Substitute.For<IConfiguration>();
        _cacheService = Substitute.For<ICacheService>();
        _restClient = Substitute.For<IRestClient>();  // Mocking IRestClient

        // Mock behavior for IRestClient.ExecuteAsync (if necessary)
        var mockResponse = new RestResponse { Content = "mockContent" };
        _restClient.ExecuteAsync(Arg.Any<RestRequest>()).Returns(Task.FromResult(mockResponse));

        // Instantiate SchemaService with mocks
        _schemaService = new SchemaService(_configuration, _cacheService, _restClient);
        
        _context = Substitute.For<IContext>();
        var state = new ExpandoObject();
        _context.State.Returns(state);
        var roles = new Dictionary<string, dynamic>();
        _context.Roles.Returns(roles);
    }

    [TestMethod]
    public async Task GetAllAsync_Should_Return_JsonResult_When_Action_Not_Skipped()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        SchemaService.SchemaIds = new List<string> { "first.schema.json", "second.schema.json" };

        _context.State.Pagination = new ExpandoObject();
        _context.State.Pagination.Page = 1;
        _context.State.Pagination.PerPage = 10;
        _context.State.Lang = "en";
        _cacheService.TryGetCacheValueAsync("first.schema.json", out Arg.Any<string>())
            .Returns(call =>
        {
            call[1] = "cachedValue";
            return Task.FromResult(true); 
        });
        
        // Act
        await _schemaService.GetAllAsync(_context, cancellationToken);

        // Assert
        var result = JsonConvert.DeserializeObject<PaginatedCollection>((string)_context.Result!)!;
        Assert.AreEqual(2, result.TotalCount);
        result.Records[0].ToString()!.Should().BeEquivalentTo("cachedValue");
        result.Records[1].ToString()!.Should().BeEquivalentTo("mockContent");
    }

    [TestMethod]
    public async Task GetByIdAsync_Should_Throw_Exception_When_SchemaId_Not_Found()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        SchemaService.SchemaIds = new List<string> { "schema1" };

        _context.State.Id = "invalidSchema";
        _context.State.Lang = "en";

        // Act & Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => 
            _schemaService.GetByIdAsync(_context, cancellationToken));
    }

    [TestMethod]
    public async Task GetByIdAsync_Should_Return_JsonSchema_When_SchemaId_Is_Found_And_Lang_IsNull()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        SchemaService.SchemaIds = new List<string> { "first.schema.json" };

        _context.State.Id = "first.schema.json";

        _cacheService.TryGetCacheValueAsync("first.schema.json", out Arg.Any<string>())
            .Returns(call =>
            {
                call[1] = "cachedValue";
                return Task.FromResult(true); 
            });

        // Act
        await _schemaService.GetByIdAsync(_context, cancellationToken);

        // Assert
        Assert.AreEqual("cachedValue", _context.Result);
    }
    
    [TestMethod]
    public async Task GetByIdAsync_Should_Return_JsonSchema_When_SchemaId_Is_Found_And_Lang_IsNotNull()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        SchemaService.SchemaIds = new List<string> { "first.schema.json" };

        _context.State.Id = "first.schema.json";
        _context.State.Lang = "en";

        _cacheService.TryGetCacheValueAsync("first.en.json", out Arg.Any<string>())
            .Returns(call =>
            {
                call[1] = "cachedValue";
                return Task.FromResult(true); 
            });

        // Act
        await _schemaService.GetByIdAsync(_context, cancellationToken);

        // Assert
        Assert.AreEqual("cachedValue", _context.Result);
    }

    [TestMethod]
    public async Task CreateAsync_Should_Add_SchemaId_To_SchemaIds_List()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        _context.State.Id = "newSchema";
        
        // Act
        await _schemaService.CreateAsync(_context, cancellationToken);

        // Assert
        Assert.IsTrue(SchemaService.SchemaIds.Contains("newSchema"));
    }
}