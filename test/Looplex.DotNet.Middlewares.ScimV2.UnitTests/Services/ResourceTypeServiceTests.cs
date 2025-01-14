using System.Dynamic;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Middlewares.ScimV2.Services;
using Newtonsoft.Json;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Services;

[TestClass]
public class ResourceTypeServiceTests
{
    private IScimV2Context _context = null!;
    private ResourceTypeService _resourceTypeService = null!;
    private CancellationToken _cancellationToken;

    [TestInitialize]
    public void Setup()
    {
        _context = Substitute.For<IScimV2Context>();
        _resourceTypeService = new ResourceTypeService(new DefaultExtensionPointOrchestrator());
        _cancellationToken = CancellationToken.None;

        // Set up dynamic state in context
        dynamic state = new ExpandoObject();
        _context.State.Returns(state);
        var roles = new Dictionary<string, dynamic>();
        _context.Roles.Returns(roles);
    }

    [TestMethod]
    public async Task GetAllAsync_ShouldPaginateAndReturnResourceTypes()
    {
        // Arrange
        ResourceTypeService.ResourceTypes = new List<ResourceType>
        {
            new ResourceType { Id = "resource1", Name = "User", Schemas = new[] { "schema1" }, Endpoint = "Users", Schema = "urn:ietf:params:scim:schemas:core:2.0:User" },
            new ResourceType { Id = "resource2", Name = "Group", Schemas = new[] { "schema2" }, Endpoint = "Groups", Schema = "urn:ietf:params:scim:schemas:core:2.0:Group" },
        };
        _context.State.Pagination = new ExpandoObject();
        _context.State.Pagination.StartIndex = 1;
        _context.State.Pagination.ItemsPerPage = 10;

        // Act
        await _resourceTypeService.GetAllAsync(_context, _cancellationToken);

        // Assert
        var result = JsonConvert.DeserializeObject<ListResponse>((string)_context.Result!)!;
        Assert.AreEqual(2, result.Resources.Count);
        Assert.AreEqual(2, result.TotalResults);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnResourceType_WhenIdExists()
    {
        // Arrange
        ResourceTypeService.ResourceTypes = new List<ResourceType>
        {
            new ResourceType { Id = "resource1", Name = "User", Schemas = new[] { "schema1" }, Endpoint = "Users", Schema = "urn:ietf:params:scim:schemas:core:2.0:User" }
        };
        
        _context.RouteValues = new Dictionary<string, object?>()
        {
            { "resourceTypeId", "resource1" }
        };

        // Act
        await _resourceTypeService.GetByIdAsync(_context, _cancellationToken);

        // Assert
        Assert.IsNotNull(_context.Roles["ResourceType"]);
        var resourceType = _context.Roles["ResourceType"];
        Assert.AreEqual("resource1", resourceType.Id);
        Assert.AreEqual("User", resourceType.Name);
        resourceType = JsonConvert.DeserializeObject<ResourceType>((string)_context.Result!)!;
        Assert.AreEqual("resource1", resourceType.Id);
        Assert.AreEqual("User", resourceType.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task GetByIdAsync_ShouldThrowException_WhenIdDoesNotExist()
    {
        // Arrange
        ResourceTypeService.ResourceTypes = new List<ResourceType>();
        
        _context.RouteValues = new Dictionary<string, object?>()
        {
            { "resourceTypeId", "invalidId" }
        };
        
        // Act
        await _resourceTypeService.GetByIdAsync(_context, _cancellationToken);
    }

    [TestMethod]
    public async Task CreateAsync_ShouldAddResourceTypeToList()
    {
        // Arrange
        var resourceType = new ResourceType
        {
            Id = "newResource",
            Name = "New Resource",
            Schemas = new[] { "schema3" },
            Endpoint = "NewEndpoint",
            Schema = "urn:ietf:params:scim:schemas:core:2.0:NewResource"
        };
        _context.State.ResourceType = resourceType;

        var count = ResourceTypeService.ResourceTypes.Count;

        // Act
        await _resourceTypeService.CreateAsync(_context, _cancellationToken);

        // Assert
        Assert.AreEqual(1 + count, ResourceTypeService.ResourceTypes.Count);
        Assert.AreEqual("newResource", ResourceTypeService.ResourceTypes.Last().Id);
    }
}