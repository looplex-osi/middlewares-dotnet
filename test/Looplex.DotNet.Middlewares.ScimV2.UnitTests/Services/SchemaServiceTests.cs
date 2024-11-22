using System.Dynamic;
using FluentAssertions;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
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
    private IJsonSchemaProvider _jsonSchemaProvider = null!;
    private IContext _context = null!;

    [TestInitialize]
    public void Setup()
    {
        // Mock dependencies
        _jsonSchemaProvider = Substitute.For<IJsonSchemaProvider>();

        // Instantiate SchemaService with mocks
        _schemaService = new SchemaService(_jsonSchemaProvider);
        
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
        var mockResponse = new RestResponse { Content = "mockContent" };
        SchemaService.SchemaIds = new List<string>
        {
            "first.schema.json",
            "second.schema.json"
        };

        _context.State.Pagination = new ExpandoObject();
        _context.State.Pagination.StartIndex = 1;
        _context.State.Pagination.ItemsPerPage = 10;
        _context.State.Lang = "en";
        _jsonSchemaProvider.ResolveJsonSchemasAsync(Arg.Any<List<string>>(), Arg.Any<string>())
            .Returns(["mockContent1", "mockContent2"]);
        
        // Act
        await _schemaService.GetAllAsync(_context, CancellationToken.None);

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
        var cancellationToken = CancellationToken.None;

        _context.State.Id = "newSchema";
        
        // Act
        await _schemaService.CreateAsync(_context, cancellationToken);

        // Assert
        Assert.IsTrue(SchemaService.SchemaIds.Contains("newSchema"));
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
}