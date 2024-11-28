using FluentAssertions;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Providers;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using RestSharp;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Providers;

[TestClass]
public class JsonSchemaProviderTests
{
    private JsonSchemaProvider _jsonSchemaProvider = null!;
    private IConfiguration _configuration = null!;
    private IRedisService _redisService = null!;
    private IRestClient _restClient = null!;

    [TestInitialize]
    public void Setup()
    {
        // Mock dependencies
        _configuration = Substitute.For<IConfiguration>();
        _redisService = Substitute.For<IRedisService>();
        _restClient = Substitute.For<IRestClient>(); 

        // Instantiate JsonSchemaProvider with mocks
        _jsonSchemaProvider = new JsonSchemaProvider(_configuration, _redisService, _restClient);
    }

    [TestMethod]
    public async Task ResolveJsonSchemasAsync_Should_Return_Schemas()
    {
        // Arrange
        var mockResponse = new RestResponse { Content = "mockContent" };
        _restClient.ExecuteAsync(Arg.Any<RestRequest>()).Returns(Task.FromResult(mockResponse));
        var cancellationToken = CancellationToken.None;
        var schemaIds = new List<string>
        {
            "first.schema.json",
            "second.schema.json"
        };
        var lang = "en";
        var ocpApimSubscriptionKey = "ocpApimSubscriptionKey";
        _redisService.GetAsync("first.en.json")
            .Returns("cachedValue1");
        
        // Act
        var schemas = await _jsonSchemaProvider.ResolveJsonSchemasAsync(ocpApimSubscriptionKey, schemaIds, lang);

        // Assert
        Assert.AreEqual(2, schemas.Count);
        schemas[0].Should().BeEquivalentTo("cachedValue1");
        schemas[1].Should().BeEquivalentTo("mockContent");
    }

    [TestMethod]
    public async Task ResolveJsonSchemasAsync_Should_Return_Schemas_FallbackToNotLocalizedSchema()
    {
        var mockResponse = new RestResponse { Content = null };
        _restClient.ExecuteAsync(Arg.Any<RestRequest>()).Returns(Task.FromResult(mockResponse));
        // Arrange
        var cancellationToken = CancellationToken.None;
        var schemaIds = new List<string>
        {
            "first.schema.json"
        };
        var lang = "en";
        var ocpApimSubscriptionKey = "ocpApimSubscriptionKey";
        _redisService.GetAsync("first.schema.json")
            .Returns("cachedValue");
        
        // Act
        var schemas = await _jsonSchemaProvider.ResolveJsonSchemasAsync(ocpApimSubscriptionKey, schemaIds, lang);

        // Assert
        Assert.AreEqual(1, schemas.Count);
        schemas[0].ToString()!.Should().BeEquivalentTo("cachedValue");
    }

    

    [TestMethod]
    public async Task ResolveJsonSchemaAsync_Should_Return_Schema_When_SchemaId_Is_Found_And_Lang_IsNull()
    {
        // Arrange
        var schemaId = "first.schema.json";
        var ocpApimSubscriptionKey = "ocpApimSubscriptionKey";
        
        _redisService.GetAsync("first.schema.json")
            .Returns("cachedValue");
        
        // Act
        var schema = await _jsonSchemaProvider.ResolveJsonSchemaAsync(ocpApimSubscriptionKey, schemaId, null);

        // Assert
        Assert.AreEqual("cachedValue", schema);
    }

    [TestMethod]
    public async Task GetByIdAsync_Should_Return_JsonSchema_When_SchemaId_Is_Found_And_Lang_IsNotNull()
    {
        // Arrange
        var schemaId = "first.schema.json";
        var lang = "en";
        var ocpApimSubscriptionKey = "ocpApimSubscriptionKey";
        
        _redisService.GetAsync("first.en.json")
            .Returns("cachedValue");

        // Act
        var schema = await _jsonSchemaProvider.ResolveJsonSchemaAsync(ocpApimSubscriptionKey, schemaId, lang);

        // Assert
        Assert.AreEqual("cachedValue", schema);
    }

    [TestMethod]
    public async Task ResolveJsonSchemasAsync_ShouldReturnJsonSchemas_WhenSchemasAreFound()
    {
        // Arrange
        var schemaIds = new List<string> { "schema1", "schema2" };
        var jsonSchema1 = "{ \"type\": \"schema1\" }";
        var jsonSchema2 = "{ \"type\": \"schema2\" }";

        _redisService.GetAsync("schema1")
            .Returns(jsonSchema1);
        _redisService.GetAsync("schema2")
            .Returns(jsonSchema2);
        
        // Act
        var result = await _jsonSchemaProvider.ResolveJsonSchemasAsync("", schemaIds);

        // Assert
        result.Should().BeEquivalentTo(new List<string> { jsonSchema1, jsonSchema2 });
    }
}