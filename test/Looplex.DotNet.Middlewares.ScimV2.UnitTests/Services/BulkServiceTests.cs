using System.Dynamic;
using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;
using Looplex.DotNet.Middlewares.ScimV2.Services;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Newtonsoft.Json;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Services;

[TestClass]
public class BulkServiceTests
{
    private ServiceProviderConfiguration _serviceProviderConfiguration = null!;
    private List<ResourceMap> _map = null!;
    private IContext _context = null!;
    private IContext _operationContext = null!;
    private ICrudService _service = null!;
    private BulkResponse _bulkResponse = null!;
    private Dictionary<string, string> _bulkIdCrossReference = null!;
    
    [TestInitialize]
    public void Setup()
    {
        _map = new List<ResourceMap>
        {
            new ResourceMap { Resource = "Users", Type = typeof(User), Service = typeof(IUserService) },
            new ResourceMap { Resource = "Groups", Type = typeof(Group), Service = typeof(IGroupService) }
        };
        _serviceProviderConfiguration = Substitute.For<ServiceProviderConfiguration>();
        _serviceProviderConfiguration.Map.Returns(_map);
        
        _context = Substitute.For<IContext>();
        _operationContext = Substitute.For<IContext>();
        _service = Substitute.For<ICrudService>();
        _bulkResponse = new BulkResponse { Operations = new List<BulkResponseOperation>() };
        _bulkIdCrossReference = new Dictionary<string, string>();

        // Set up dynamic state in contexts
        dynamic state = new ExpandoObject();
        _context.State.Returns(state);
        _operationContext.State.Returns(new ExpandoObject());
    }

    [TestMethod]
    public void GetResourceMap_ShouldReturnResourceMapAndGuid_WhenPathHasGuidForNonPostMethod()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Path = "/Users/123e4567-e89b-12d3-a456-426614174000",
            Method = Method.Put
        };

        // Act
        var (resourceMap, guid) = BulkService.GetResourceMap(operation, _serviceProviderConfiguration);

        // Assert
        Assert.IsNotNull(resourceMap);
        Assert.AreEqual("Users", resourceMap.Resource);
        Assert.AreEqual(Guid.Parse("123e4567-e89b-12d3-a456-426614174000"), guid);
    }

    [TestMethod]
    public void GetResourceMap_ShouldReturnResourceMapAndNullGuid_WhenPathIsOnlyResourceForPostMethod()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Path = "/Groups",
            Method = Method.Post
        };

        // Act
        var (resourceMap, guid) = BulkService.GetResourceMap(operation, _serviceProviderConfiguration);

        // Assert
        Assert.IsNotNull(resourceMap);
        Assert.AreEqual("Groups", resourceMap.Resource);
        Assert.IsNull(guid);
    }

    [TestMethod]
    public void GetResourceMap_ShouldThrowError_WhenPathHasNoGuidForNonPostMethod()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Path = "/Users",
            Method = Method.Delete
        };

        // Act & Assert
        var ex = Assert.ThrowsException<Error>(() =>
            BulkService.GetResourceMap(operation, _serviceProviderConfiguration));

        Assert.AreEqual("Path /Users should refer to a specific resource when method is Delete", ex.Message);
        Assert.AreEqual(ErrorScimType.InvalidPath, ex.ScimType);
        Assert.AreEqual(HttpStatusCode.BadRequest.ToString(), ex.Status);
    }

    [TestMethod]
    public void GetResourceMap_ShouldThrowError_WhenPathHasInvalidGuid()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Path = "/Users/invalid-guid",
            Method = Method.Patch
        };

        // Act & Assert
        var ex = Assert.ThrowsException<Error>(() =>
            BulkService.GetResourceMap(operation, _serviceProviderConfiguration));

        Assert.AreEqual("Resource identifier invalid-guid is not valid", ex.Message);
        Assert.AreEqual(ErrorScimType.InvalidValue, ex.ScimType);
        Assert.AreEqual(HttpStatusCode.BadRequest.ToString(), ex.Status);
    }

    [TestMethod]
    public void GetResourceMap_ShouldThrowError_WhenResourceMapDoesNotExist()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Path = "/NonExistentResource/123e4567-e89b-12d3-a456-426614174000",
            Method = Method.Patch
        };

        // Act & Assert
        var ex = Assert.ThrowsException<Error>(() =>
            BulkService.GetResourceMap(operation, _serviceProviderConfiguration));

        Assert.AreEqual("Path NonExistentResource does not exist", ex.Message);
        Assert.AreEqual(ErrorScimType.InvalidPath, ex.ScimType);
        Assert.AreEqual(HttpStatusCode.BadRequest.ToString(), ex.Status);
    }
    
    [TestMethod]
    public void ValidateOperation_ShouldThrowError_WhenDataIsNullAndMethodIsNotDelete()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Put,
            Data = null,
            Path = ""
        };

        // Act & Assert
        var ex = Assert.ThrowsException<Error>(() => 
            BulkService.ValidateOperation(operation));

        Assert.AreEqual($"Data should have value for method {operation.Method}", ex.Message);
        Assert.AreEqual(ErrorScimType.InvalidValue, ex.ScimType);
        Assert.AreEqual(HttpStatusCode.BadRequest.ToString(), ex.Status);
    }

    [TestMethod]
    public void ValidateOperation_ShouldThrowError_WhenBulkIdIsNullAndMethodIsPost()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Post,
            BulkId = null,
            Data = new { Name = "Test Data" },  // Include valid data for Post
            Path = ""
        };

        // Act & Assert
        var ex = Assert.ThrowsException<Error>(() => 
            BulkService.ValidateOperation(operation));

        Assert.AreEqual($"BulkId should have value for method {operation.Method}", ex.Message);
        Assert.AreEqual(ErrorScimType.InvalidValue, ex.ScimType);
        Assert.AreEqual(HttpStatusCode.BadRequest.ToString(), ex.Status);
    }

    [TestMethod]
    public void ValidateOperation_ShouldNotThrowError_WhenDataIsProvidedForPut()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Put,
            Data = new { Name = "Valid Data" },
            Path = ""
        };

        // Act & Assert
        BulkService.ValidateOperation(operation);  // Should not throw an exception
    }

    [TestMethod]
    public void ValidateBulkIdsUniqueness_ShouldThrowError_WhenBulkIdsAreNotUnique()
    {
        // Arrange
        var bulkRequest = new BulkRequest
        {
            Operations = new List<BulkRequestOperation>
            {
                new BulkRequestOperation { 
                    BulkId = "id1", 
                    Method = Method.Post,
                    Path = "" },
                new BulkRequestOperation { 
                    BulkId = "id1", 
                    Method = Method.Put,
                    Path = "" },
                new BulkRequestOperation { 
                    BulkId = "id2", 
                    Method = Method.Post,
                    Path = "" }
            }
        };

        // Act & Assert
        var ex = Assert.ThrowsException<Error>(() =>
            BulkService.ValidateBulkIdsUniqueness(bulkRequest));

        Assert.AreEqual("BulkIds id1 must be unique", ex.Message);
        Assert.AreEqual(ErrorScimType.Uniqueness, ex.ScimType);
        Assert.AreEqual(HttpStatusCode.BadRequest.ToString(), ex.Status);
    }

    [TestMethod]
    public void ValidateBulkIdsUniqueness_ShouldNotThrowError_WhenBulkIdsAreUnique()
    {
        // Arrange
        var bulkRequest = new BulkRequest
        {
            Operations = new List<BulkRequestOperation>
            {
                new BulkRequestOperation { 
                    BulkId = "id1", 
                    Method = Method.Post,
                    Path = "" },
                new BulkRequestOperation { 
                    BulkId = "id2", 
                    Method = Method.Put,
                    Path = "" },
                new BulkRequestOperation { 
                    BulkId = "id3", 
                    Method = Method.Patch,
                    Path = "" }
            }
        };

        // Act & Assert
        BulkService.ValidateBulkIdsUniqueness(bulkRequest);  // Should not throw an exception
    }
    
    [TestMethod]
    public async Task ExecutePostMethod_ShouldSerializeDataAndAddOperationToBulkResponse()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Post,
            Path = "/Users",
            BulkId = "bulkId1",
            Data = new { Name = "John Doe" }
        };

        var resourceMap = new ResourceMap
        {
            Resource = "Users",
            Type = typeof(User),
            Service = typeof(IUserService)
        };

        // Mock service to set Result in context
        var createdId = "12345";
        _context.Result = createdId;

        // Act
        await BulkService.ExecutePostMethod(
            _context, CancellationToken.None, _operationContext, operation, _service, _bulkResponse, resourceMap, _bulkIdCrossReference);

        // Assert
        var serializedData = JsonConvert.SerializeObject(operation.Data);
        Assert.AreEqual(serializedData, _operationContext.State.Resource);

        Assert.AreEqual(1, _bulkResponse.Operations.Count);
        var responseOperation = _bulkResponse.Operations[0];
        Assert.AreEqual(Method.Post, responseOperation.Method);
        Assert.AreEqual("/Users", responseOperation.Path);
        Assert.AreEqual("Users/12345", responseOperation.Location);
        Assert.AreEqual((int)HttpStatusCode.Created, responseOperation.Status);

        Assert.AreEqual(createdId, _bulkIdCrossReference[operation.BulkId]);
        await _service.Received(1).CreateAsync(_context, CancellationToken.None);
    }

    [TestMethod]
    public async Task ExecutePostMethod_ShouldHandleNullData()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Post,
            Path = "/Users",
            BulkId = "bulkId2",
            Data = null
        };

        var resourceMap = new ResourceMap
        {
            Resource = "Users",
            Type = typeof(User),
            Service = typeof(IUserService)
        };

        // Mock service to set Result in context
        var createdId = "67890";
        _context.Result = createdId;

        // Act
        await BulkService.ExecutePostMethod(
            _context, CancellationToken.None, _operationContext, operation, _service, _bulkResponse, resourceMap, _bulkIdCrossReference);

        // Assert
        Assert.AreEqual("null", _operationContext.State.Resource);

        Assert.AreEqual(1, _bulkResponse.Operations.Count);
        var responseOperation = _bulkResponse.Operations[0];
        Assert.AreEqual(Method.Post, responseOperation.Method);
        Assert.AreEqual("/Users", responseOperation.Path);
        Assert.AreEqual("Users/67890", responseOperation.Location);
        Assert.AreEqual((int)HttpStatusCode.Created, responseOperation.Status);

        Assert.AreEqual(createdId, _bulkIdCrossReference[operation.BulkId]);
        await _service.Received(1).CreateAsync(_context, CancellationToken.None);
    }
}