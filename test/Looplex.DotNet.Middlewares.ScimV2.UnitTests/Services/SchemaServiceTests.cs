using System.Dynamic;
using FluentAssertions;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Middlewares.ScimV2.Services;
using Newtonsoft.Json;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Services;

[TestClass]
public class SchemaServiceTests
{
    private SchemaService _schemaService = null!;
    private IJsonSchemaProvider _jsonSchemaProvider = null!;
    private IScimV2Context _context = null!;

    [TestInitialize]
    public void Setup()
    {
        // Mock dependencies
        _jsonSchemaProvider = Substitute.For<IJsonSchemaProvider>();

        // Instantiate SchemaService with mocks
        _schemaService = new(new DefaultExtensionPointOrchestrator(), _jsonSchemaProvider);
        
        _context = Substitute.For<IScimV2Context>();
        var state = new ExpandoObject();
        _context.State.Returns(state);
        var roles = new Dictionary<string, dynamic>();
        _context.Roles.Returns(roles);
    }

    [TestMethod]
    public async Task GetAllAsync_Should_Return_JsonResult_When_Action_Not_Skipped()
    {
        // Arrange
        SchemaService.SchemaIds = new List<string>
        {
            "first.schema.json",
            "second.schema.json"
        };

        _context.State.Pagination = new ExpandoObject();
        _context.State.Pagination.StartIndex = 1;
        _context.State.Pagination.ItemsPerPage = 10;
        _context.State.CancellationToken = CancellationToken.None;
        _context.Headers = new Dictionary<string, string>
        {
            {
                "Ocp-Apim-Subscription-Key", "key"
            },
            {
                "Lang", "en"
            }
        };
        _jsonSchemaProvider.ResolveJsonSchemasAsync(Arg.Any<IScimV2Context>(), Arg.Any<List<string>>(), Arg.Any<string>())
            .Returns(["mockContent1", "mockContent2"]);
        
        // Act
        await _schemaService.GetAllAsync(_context);

        // Assert
        var result = JsonConvert.DeserializeObject<ListResponse>((string)_context.Result!)!;
        Assert.AreEqual(2, result.TotalResults);
        result.Resources[0].ToString()!.Should().BeEquivalentTo("mockContent1");
        result.Resources[1].ToString()!.Should().BeEquivalentTo("mockContent2");
    }

    [TestMethod]
    public async Task CreateAsync_Should_Add_SchemaId_To_SchemaIds_List()
    {
        // Arrange
        _context.State.CancellationToken = CancellationToken.None;
        _context.RouteValues = new Dictionary<string, object?>()
        {
            { "schemaId", "newSchema" }
        };
        
        // Act
        await _schemaService.CreateAsync(_context);

        // Assert
        Assert.IsTrue(SchemaService.SchemaIds.Contains("newSchema"));
    }

    [TestMethod]
    [ExpectedException(typeof(Error))]

    public async Task CreateAsync_Should_ThrowError()
    {
        // Arrange
        _context.State.CancellationToken = CancellationToken.None;
        _context.RouteValues = new Dictionary<string, object?>()
        {
            { "schemaId", "" }
        };
        
        // Act & Assert
        await _schemaService.CreateAsync(_context);
    }
    
    [TestMethod]
    public async Task GetByIdAsync_Should_Throw_Exception_When_SchemaId_Not_Found()
    {
        // Arrange
        SchemaService.SchemaIds = new List<string> { "schema1" };

        _context.State.CancellationToken = CancellationToken.None;
        _context.RouteValues = new Dictionary<string, object?>()
        {
            { "schemaId", "invalidSchema" }
        };
        _context.Headers = new Dictionary<string, string>
        {
            {
                "Ocp-Apim-Subscription-Key", "key"
            }
        };
        
        // Act & Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            _schemaService.GetByIdAsync(_context));
    }
}