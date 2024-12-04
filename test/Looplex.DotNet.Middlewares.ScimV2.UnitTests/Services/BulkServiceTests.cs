using System.Dynamic;
using System.Net;
using FluentAssertions;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;
using Looplex.DotNet.Middlewares.ScimV2.Services;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;

namespace Looplex.DotNet.Middlewares.ScimV2.UnitTests.Services;

[TestClass]
public class BulkServiceTests
{
    private ServiceProviderConfiguration _serviceProviderConfiguration = null!;
    private List<ResourceMap> _map = null!;
    private IScimV2Context _context = null!;
    private IContext _operationContext = null!;
    private ICrudService _service = null!;
    private BulkResponse _bulkResponse = null!;
    private Dictionary<string, string> _bulkIdCrossReference = null!;
    private IServiceProvider _serviceProvider = null!;
    private IContextFactory _contextFactory = null!;
    private IConfiguration _configuration = null!;
    private IJsonSchemaProvider _jsonSchemaProvider = null!;
    
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
        
        _context = Substitute.For<IScimV2Context>();
        _operationContext = Substitute.For<IContext>();
        _service = Substitute.For<ICrudService>();
        _configuration = Substitute.For<IConfiguration>();
        _configuration["JsonSchemaIdForBulkOperation"] = "bulkJsonSchemaId";
        _jsonSchemaProvider = Substitute.For<IJsonSchemaProvider>();
        _bulkResponse = new BulkResponse { Operations = new List<BulkResponseOperation>() };
        _bulkIdCrossReference = new Dictionary<string, string>
        {
            { "id1", "123e4567-e89b-12d3-a456-426614174000" },
            { "id2", "456e1234-e89b-12d3-a456-426614174111" }
        };
        // Set up dynamic state in contexts
        dynamic state = new ExpandoObject();
        _context.State.Returns(state);
        _context.Headers = [];
        _operationContext.State.Returns(new ExpandoObject());
        
        _serviceProvider = Substitute.For<IServiceProvider>();
        _contextFactory = Substitute.For<IContextFactory>();
        
        _serviceProvider.GetService(typeof(ServiceProviderConfiguration)).Returns(_serviceProviderConfiguration);
        _serviceProvider.GetService(Arg.Is<Type>(t => typeof(ICrudService).IsAssignableFrom(t))).Returns(_service);
        _contextFactory.Create(Arg.Any<IEnumerable<string>>()).Returns(_operationContext);
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
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ex.Status);
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
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ex.Status);
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
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ex.Status);
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
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ex.Status);
    }

    [TestMethod]
    public void ValidateOperation_ShouldThrowError_WhenBulkIdIsNullAndMethodIsPost()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Post,
            BulkId = null,
            Data = JsonConvert
                .DeserializeObject<JToken>(JsonConvert
                    .SerializeObject(new { Name = "Test Data" })),  // Include valid data for Post
            Path = ""
        };

        // Act & Assert
        var ex = Assert.ThrowsException<Error>(() => 
            BulkService.ValidateOperation(operation));

        Assert.AreEqual($"BulkId should have value for method {operation.Method}", ex.Message);
        Assert.AreEqual(ErrorScimType.InvalidValue, ex.ScimType);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ex.Status);
    }

    [TestMethod]
    public void ValidateOperation_ShouldNotThrowError_WhenDataIsProvidedForPut()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Put,
            Data = JsonConvert
                .DeserializeObject<JToken>(JsonConvert
                    .SerializeObject(new { Name = "Test Data" })),
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
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ex.Status);
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
            Data = JsonConvert
                .DeserializeObject<JToken>(JsonConvert
                    .SerializeObject(new { Name = "Test Data" }))
        };

        var resourceMap = new ResourceMap
        {
            Resource = "Users",
            Type = typeof(User),
            Service = typeof(IUserService)
        };

        // Mock service to set Result in context
        var createdId = "12345";
        _operationContext.Result = createdId;

        // Act
        await BulkService.ExecutePostMethod(
            _operationContext, operation, _service, _bulkResponse, resourceMap, CancellationToken.None);

        // Assert
        var serializedData = JsonConvert.SerializeObject(operation.Data);
        Assert.AreEqual(serializedData, _operationContext.State.Resource);

        Assert.AreEqual(1, _bulkResponse.Operations.Count);
        var responseOperation = _bulkResponse.Operations[0];
        Assert.AreEqual(Method.Post, responseOperation.Method);
        Assert.AreEqual("/Users", responseOperation.Path);
        Assert.AreEqual("Users/12345", responseOperation.Location);
        Assert.AreEqual((int)HttpStatusCode.Created, responseOperation.Status);

        await _service.Received(1).CreateAsync(_operationContext, CancellationToken.None);
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
        _operationContext.Result = createdId;
        // Act
        await BulkService.ExecutePostMethod(_operationContext, operation, _service, _bulkResponse, resourceMap, CancellationToken.None);

        // Assert
        Assert.AreEqual("null", _operationContext.State.Resource);

        Assert.AreEqual(1, _bulkResponse.Operations.Count);
        var responseOperation = _bulkResponse.Operations[0];
        Assert.AreEqual(Method.Post, responseOperation.Method);
        Assert.AreEqual("/Users", responseOperation.Path);
        Assert.AreEqual("Users/67890", responseOperation.Location);
        Assert.AreEqual((int)HttpStatusCode.Created, responseOperation.Status);

        await _service.Received(1).CreateAsync(_operationContext, CancellationToken.None);
    }
    
    [TestMethod]
    public async Task ExecutePutMethod_ShouldSerializeDataAndAddOperationToBulkResponse()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Put,
            Path = "/Users/123e4567-e89b-12d3-a456-426614174000",
            Data = JsonConvert
                .DeserializeObject<JToken>(JsonConvert
                    .SerializeObject(new { Name = "Jane Doe" }))
        };

        var resourceMap = new ResourceMap
        {
            Resource = "Users",
            Type = typeof(User),
            Service = typeof(IUserService)
        };

        var resourceUniqueId = Guid.Parse("123e4567-e89b-12d3-a456-426614174000");

        // Mock UpdateAsync to set Result in operation context
        var updatedId = "123e4567-e89b-12d3-a456-426614174000";
        _operationContext.Result = updatedId;

        // Act
        await BulkService.ExecutePutMethod(
            _operationContext, operation, _service, _bulkResponse, resourceMap, resourceUniqueId, CancellationToken.None);

        // Assert
        var serializedData = JsonConvert.SerializeObject(operation.Data);
        Assert.AreEqual(serializedData, _operationContext.State.Resource);
        Assert.AreEqual(resourceUniqueId.ToString(), _operationContext.State.Id);

        Assert.AreEqual(1, _bulkResponse.Operations.Count);
        var responseOperation = _bulkResponse.Operations[0];
        Assert.AreEqual(Method.Put, responseOperation.Method);
        Assert.AreEqual("/Users/123e4567-e89b-12d3-a456-426614174000", responseOperation.Path);
        Assert.AreEqual("Users/123e4567-e89b-12d3-a456-426614174000", responseOperation.Location);
        Assert.AreEqual((int)HttpStatusCode.NoContent, responseOperation.Status);

        await _service.Received(1).UpdateAsync(_operationContext, CancellationToken.None);
    }

    [TestMethod]
    public async Task ExecutePutMethod_ShouldHandleNullData()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Put,
            Path = "/Groups/456e1234-e89b-12d3-a456-426614174111",
            Data = null
        };

        var resourceMap = new ResourceMap
        {
            Resource = "Groups",
            Type = typeof(Group),
            Service = typeof(IGroupService)
        };

        var resourceUniqueId = Guid.Parse("456e1234-e89b-12d3-a456-426614174111");

        // Mock UpdateAsync to set Result in operation context
        var updatedId = "456e1234-e89b-12d3-a456-426614174111";
        _operationContext.Result = updatedId;

        // Act
        await BulkService.ExecutePutMethod(
            _operationContext, operation, _service, _bulkResponse, resourceMap, resourceUniqueId, CancellationToken.None);

        // Assert
        Assert.AreEqual("null", _operationContext.State.Resource);
        Assert.AreEqual(resourceUniqueId.ToString(), _operationContext.State.Id);

        Assert.AreEqual(1, _bulkResponse.Operations.Count);
        var responseOperation = _bulkResponse.Operations[0];
        Assert.AreEqual(Method.Put, responseOperation.Method);
        Assert.AreEqual("/Groups/456e1234-e89b-12d3-a456-426614174111", responseOperation.Path);
        Assert.AreEqual("Groups/456e1234-e89b-12d3-a456-426614174111", responseOperation.Location);
        Assert.AreEqual((int)HttpStatusCode.NoContent, responseOperation.Status);

        await _service.Received(1).UpdateAsync(_operationContext, CancellationToken.None);
    }
    
    [TestMethod]
    public async Task ExecutePatchMethod_ShouldSerializeOperationsAndAddOperationToBulkResponse()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Patch,
            Path = "/Users/123e4567-e89b-12d3-a456-426614174000",
            Data = JsonConvert
                .DeserializeObject<JToken>(JsonConvert
                    .SerializeObject(new { Patch = "updateData" }))
        };

        var resourceMap = new ResourceMap
        {
            Resource = "Users",
            Type = typeof(User),
            Service = typeof(IUserService)
        };

        var resourceUniqueId = Guid.Parse("123e4567-e89b-12d3-a456-426614174000");

        // Mock UpdateAsync to set Result in operation context
        var updatedId = "123e4567-e89b-12d3-a456-426614174000";
        _operationContext.Result = updatedId;

        // Act
        await BulkService.ExecutePatchMethod(
            _operationContext, operation, _service, _bulkResponse, resourceMap, resourceUniqueId, CancellationToken.None);

        // Assert
        var serializedData = JsonConvert.SerializeObject(operation.Data);
        Assert.AreEqual(serializedData, _operationContext.State.Operations);
        Assert.AreEqual(resourceUniqueId.ToString(), _operationContext.State.Id);

        Assert.AreEqual(1, _bulkResponse.Operations.Count);
        var responseOperation = _bulkResponse.Operations[0];
        Assert.AreEqual(Method.Patch, responseOperation.Method);
        Assert.AreEqual("/Users/123e4567-e89b-12d3-a456-426614174000", responseOperation.Path);
        Assert.AreEqual("Users/123e4567-e89b-12d3-a456-426614174000", responseOperation.Location);
        Assert.AreEqual((int)HttpStatusCode.NoContent, responseOperation.Status);

        await _service.Received(1).UpdateAsync(_operationContext, CancellationToken.None);
    }

    [TestMethod]
    public async Task ExecutePatchMethod_ShouldHandleNullData()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Patch,
            Path = "/Groups/456e1234-e89b-12d3-a456-426614174111",
            Data = null
        };

        var resourceMap = new ResourceMap
        {
            Resource = "Groups",
            Type = typeof(Group),
            Service = typeof(IGroupService)
        };

        var resourceUniqueId = Guid.Parse("456e1234-e89b-12d3-a456-426614174111");

        // Mock UpdateAsync to set Result in operation context
        var updatedId = "456e1234-e89b-12d3-a456-426614174111";
        _operationContext.Result = updatedId;

        // Act
        await BulkService.ExecutePatchMethod(
            _operationContext, operation, _service, _bulkResponse, resourceMap, resourceUniqueId, CancellationToken.None);

        // Assert
        Assert.AreEqual("null", _operationContext.State.Operations);
        Assert.AreEqual(resourceUniqueId.ToString(), _operationContext.State.Id);

        Assert.AreEqual(1, _bulkResponse.Operations.Count);
        var responseOperation = _bulkResponse.Operations[0];
        Assert.AreEqual(Method.Patch, responseOperation.Method);
        Assert.AreEqual("/Groups/456e1234-e89b-12d3-a456-426614174111", responseOperation.Path);
        Assert.AreEqual("Groups/456e1234-e89b-12d3-a456-426614174111", responseOperation.Location);
        Assert.AreEqual((int)HttpStatusCode.NoContent, responseOperation.Status);

        await _service.Received(1).UpdateAsync(_operationContext, CancellationToken.None);
    }
    
    [TestMethod]
    public async Task ExecuteDeleteMethod_ShouldSetIdAndAddOperationToBulkResponse()
    {
        // Arrange
        var operation = new BulkRequestOperation
        {
            Method = Method.Delete,
            Path = "/Users/123e4567-e89b-12d3-a456-426614174000"
        };

        var resourceUniqueId = Guid.Parse("123e4567-e89b-12d3-a456-426614174000");

        // Act
        await BulkService.ExecuteDeleteMethod(
            _operationContext, operation, _service, _bulkResponse, resourceUniqueId, CancellationToken.None);

        // Assert
        Assert.AreEqual(resourceUniqueId.ToString(), _operationContext.State.Id);

        Assert.AreEqual(1, _bulkResponse.Operations.Count);
        var responseOperation = _bulkResponse.Operations[0];
        Assert.AreEqual(Method.Delete, responseOperation.Method);
        Assert.AreEqual("/Users/123e4567-e89b-12d3-a456-426614174000", responseOperation.Path);
        Assert.AreEqual((int)HttpStatusCode.NoContent, responseOperation.Status);

        await _service.Received(1).DeleteAsync(_operationContext, CancellationToken.None);
    }
    
    [TestMethod]
    public void BulkIdVisitor_ShouldReplaceBulkId_WhenReferenceExists()
    {
        // Arrange
        var json = JToken.Parse("{ \"reference\": \"bulkId:id1\" }");
        var visitor = BulkService.BulkIdVisitor(_bulkIdCrossReference);

        // Act
        visitor(json["reference"]!);

        // Assert
        Assert.AreEqual("123e4567-e89b-12d3-a456-426614174000", (string)json["reference"]!);
    }

    [TestMethod]
    public void BulkIdVisitor_ShouldNotModifyNode_WhenNoBulkIdPrefix()
    {
        // Arrange
        var json = JToken.Parse("{ \"reference\": \"someOtherId\" }");
        var originalValue = (string)json["reference"]!;
        var visitor = BulkService.BulkIdVisitor(_bulkIdCrossReference);

        // Act
        visitor(json["reference"]!);

        // Assert
        Assert.AreEqual(originalValue, (string)json["reference"]!);
    }

    [TestMethod]
    public void BulkIdVisitor_ShouldThrowError_WhenBulkIdReferenceDoesNotExist()
    {
        // Arrange
        var json = JToken.Parse("{ \"reference\": \"bulkId:unknownId\" }");
        var visitor = BulkService.BulkIdVisitor(_bulkIdCrossReference);

        // Act & Assert
        var ex = Assert.ThrowsException<Error>(() => visitor(json["reference"]!));
        Assert.AreEqual("Bulk id unknownId not defined", ex.Message);
        Assert.AreEqual(ErrorScimType.InvalidValue, ex.ScimType);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, ex.Status);
    }

    [TestMethod]
    public void BulkIdVisitor_ShouldHandleMultipleBulkIdReferences()
    {
        // Arrange
        var json = JToken.Parse("{ \"ref1\": \"bulkId:id1\", \"ref2\": \"bulkId:id2\" }");
        var visitor = BulkService.BulkIdVisitor(_bulkIdCrossReference);

        // Act
        visitor(json["ref1"]!);
        visitor(json["ref2"]!);

        // Assert
        Assert.AreEqual("123e4567-e89b-12d3-a456-426614174000", (string)json["ref1"]!);
        Assert.AreEqual("456e1234-e89b-12d3-a456-426614174111", (string)json["ref2"]!);
    }
    
    [TestMethod]
    public async Task ExecuteBulkOperationsAsync_ShouldProcessPostOperationSuccessfully()
    {
        // Arrange
        _jsonSchemaProvider
            .ResolveJsonSchemaAsync(Arg.Any<IScimV2Context>(), "bulkJsonSchemaId")
            .Returns("{}");
        var bulkRequest = new BulkRequest
        {
            Operations = new List<BulkRequestOperation>
            {
                new BulkRequestOperation
                {
                    Method = Method.Post,
                    Path = "/Users",
                    BulkId = "bulkId1",
                    Data = new JObject { { "name", "John Doe" } }
                }
            }
        };
        _context.State.Request = bulkRequest.ToJson();
        _context.Headers.Add("Ocp-Apim-Subscription-Key", "ocpApimSubscriptionKey");

        var createdId = "12345";
        
        _service.When(x => x.CreateAsync(Arg.Any<IContext>(), CancellationToken.None))
            .Do((_) =>
            {
                _operationContext.Result = createdId;
            });

        // Act
        var executeTask = new BulkService(_serviceProvider, _contextFactory, _configuration, _jsonSchemaProvider)
            .ExecuteBulkOperationsAsync(_context, CancellationToken.None);
        await executeTask;

        // Assert
        var bulkResponse = JsonConvert.DeserializeObject<BulkResponse>((string)_context.Result!)!;
        Assert.AreEqual(1, bulkResponse.Operations.Count);
        Assert.AreEqual(Method.Post, bulkResponse.Operations[0].Method);
        Assert.AreEqual("/Users", bulkResponse.Operations[0].Path);
        Assert.AreEqual("Users/12345", bulkResponse.Operations[0].Location);
        Assert.AreEqual((int)HttpStatusCode.Created, bulkResponse.Operations[0].Status);
    }

    [TestMethod]
    public async Task ExecuteBulkOperationsAsync_ShouldProcessDeleteOperationSuccessfully()
    {
        // Arrange
        _jsonSchemaProvider
            .ResolveJsonSchemaAsync(Arg.Any<IScimV2Context>(), "bulkJsonSchemaId")
            .Returns("{}");
        var bulkRequest = new BulkRequest
        {
            Operations = new List<BulkRequestOperation>
            {
                new BulkRequestOperation
                {
                    Method = Method.Delete,
                    Path = "/Users/123e4567-e89b-12d3-a456-426614174000"
                }
            }
        };
        _context.State.Request = bulkRequest.ToJson();
        _context.Headers.Add("Ocp-Apim-Subscription-Key", "ocpApimSubscriptionKey");

        // Act
        await new BulkService(_serviceProvider, _contextFactory, _configuration, _jsonSchemaProvider)
            .ExecuteBulkOperationsAsync(_context, CancellationToken.None);

        // Assert
        var bulkResponse = JsonConvert.DeserializeObject<BulkResponse>((string)_context.Result!)!;
        Assert.AreEqual(1, bulkResponse.Operations.Count);
        Assert.AreEqual(Method.Delete, bulkResponse.Operations[0].Method);
        Assert.AreEqual("/Users/123e4567-e89b-12d3-a456-426614174000", bulkResponse.Operations[0].Path);
        Assert.AreEqual((int)HttpStatusCode.NoContent, bulkResponse.Operations[0].Status);
    }

    [TestMethod]
    public async Task ExecuteBulkOperationsAsync_ShouldHandleMissingBulkIdForPostOperation()
    {
        // Arrange
        _jsonSchemaProvider
            .ResolveJsonSchemaAsync(Arg.Any<IScimV2Context>(), "bulkJsonSchemaId")
            .Returns("{}");
        var bulkRequest = new BulkRequest
        {
            Operations = new List<BulkRequestOperation>
            {
                new BulkRequestOperation
                {
                    Method = Method.Post,
                    Path = "/Users",
                    Data = new JObject { { "name", "John Doe" } }
                },
                new BulkRequestOperation
                {
                    Method = Method.Post,
                    Path = "/WillNotExecute",
                    Data = null
                }
            },
            FailOnErrors = 1
        };
        _context.State.Request = bulkRequest.ToJson();
        _context.Headers.Add("Ocp-Apim-Subscription-Key", "ocpApimSubscriptionKey");

        // Act
        await new BulkService(_serviceProvider, _contextFactory, _configuration, _jsonSchemaProvider)
            .ExecuteBulkOperationsAsync(_context, CancellationToken.None);
        
        // Assert
        var bulkResponse = JsonConvert.DeserializeObject<BulkResponse>((string)_context.Result!)!;
        bulkResponse.Operations.Count.Should().Be(1);
        var error = bulkResponse.Operations[0].Response!.ToObject<Error>()!;
        Assert.AreEqual("BulkId should have value for method Post", error.Detail);
    }

    [TestMethod]
    public async Task ExecuteBulkOperationsAsync_ShouldAddErrorToBulkResponse_WhenResourceIdentifierIsInvalid()
    {
        // Arrange
        _jsonSchemaProvider
            .ResolveJsonSchemaAsync(Arg.Any<IScimV2Context>(), "bulkJsonSchemaId")
            .Returns("{}");
        var bulkRequest = new BulkRequest
        {
            Operations = new List<BulkRequestOperation>
            {
                new BulkRequestOperation
                {
                    Method = Method.Patch,
                    Path = "/NonExistent/12345",
                    Data = new JObject { { "updateField", "value" } }
                }
            },
            FailOnErrors = 1
        };
        _context.State.Request = bulkRequest.ToJson();
        _context.Headers.Add("Ocp-Apim-Subscription-Key", "ocpApimSubscriptionKey");

        // Act
        await new BulkService(_serviceProvider, _contextFactory, _configuration, _jsonSchemaProvider)
            .ExecuteBulkOperationsAsync(_context, CancellationToken.None);

        // Assert
        var bulkResponse = JsonConvert.DeserializeObject<BulkResponse>((string)_context.Result!)!;
        Assert.AreEqual(1, bulkResponse.Operations.Count);
        Assert.AreEqual(Method.Patch, bulkResponse.Operations[0].Method);
        Assert.AreEqual("/NonExistent/12345", bulkResponse.Operations[0].Path);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, bulkResponse.Operations[0].Status);
        var error = bulkResponse.Operations[0].Response!.ToObject<Error>()!;
        Assert.AreEqual("Resource identifier 12345 is not valid", error.Detail);
    }

    [TestMethod]
    public async Task ExecuteBulkOperationsAsync_ShouldAddErrorToBulkResponse_WhenOperationFails()
    {
        // Arrange
        _jsonSchemaProvider
            .ResolveJsonSchemaAsync(Arg.Any<IScimV2Context>(), "bulkJsonSchemaId")
            .Returns("{}");
        var id = Guid.NewGuid();
        var bulkRequest = new BulkRequest
        {
            Operations = new List<BulkRequestOperation>
            {
                new BulkRequestOperation
                {
                    Method = Method.Patch,
                    Path = $"/NonExistent/{id}",
                    Data = new JObject { { "updateField", "value" } }
                }
            },
            FailOnErrors = 1
        };
        _context.State.Request = bulkRequest.ToJson();
        _context.Headers.Add("Ocp-Apim-Subscription-Key", "ocpApimSubscriptionKey");
        // Act
        await new BulkService(_serviceProvider, _contextFactory, _configuration, _jsonSchemaProvider)
            .ExecuteBulkOperationsAsync(_context, CancellationToken.None);

        // Assert
        var bulkResponse = JsonConvert.DeserializeObject<BulkResponse>((string)_context.Result!)!;
        Assert.AreEqual(1, bulkResponse.Operations.Count);
        Assert.AreEqual(Method.Patch, bulkResponse.Operations[0].Method);
        Assert.AreEqual($"/NonExistent/{id}", bulkResponse.Operations[0].Path);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, bulkResponse.Operations[0].Status);
        var error = bulkResponse.Operations[0].Response!.ToObject<Error>()!;
        Assert.AreEqual("Path NonExistent does not exist", error.Detail);
    }
    
    [TestMethod]
    public async Task ExecuteBulkOperationsAsync_ShouldProcessMultipleOperationsWithBulkIdReference()
    {
        // Arrange
        _jsonSchemaProvider
            .ResolveJsonSchemaAsync(Arg.Any<IScimV2Context>(), "bulkJsonSchemaId")
            .Returns("{}");
        var groupId = Guid.NewGuid();
        var bulkRequest = new BulkRequest
        {
            Operations = new List<BulkRequestOperation>
            {
                new BulkRequestOperation
                {
                    Method = Method.Post,
                    Path = "/Users",
                    BulkId = "bulkId1",
                    Data = new JObject { { "name", "John Doe" } }
                },
                new BulkRequestOperation
                {
                    Method = Method.Put,
                    Path = $"/Groups/{groupId}",
                    Data = new JObject { { "userReference", "bulkId:bulkId1" } }
                }
            }
        };
        _context.State.Request = bulkRequest.ToJson();
        _context.Headers.Add("Ocp-Apim-Subscription-Key", "ocpApimSubscriptionKey");

        var createdUserId = Guid.NewGuid();
        _service.When(x => x.CreateAsync(Arg.Any<IContext>(), CancellationToken.None))
            .Do((_) =>
            {
                _operationContext.Result = createdUserId.ToString();
            });

        // Act
        await new BulkService(_serviceProvider, _contextFactory, _configuration, _jsonSchemaProvider)
            .ExecuteBulkOperationsAsync(_context, CancellationToken.None);

        // Assert
        var bulkResponse = JsonConvert.DeserializeObject<BulkResponse>((string)_context.Result!)!;

        // Verify first operation was processed
        Assert.AreEqual(2, bulkResponse.Operations.Count);
        Assert.AreEqual(Method.Post, bulkResponse.Operations[0].Method);
        Assert.AreEqual("/Users", bulkResponse.Operations[0].Path);
        Assert.AreEqual($"Users/{createdUserId}", bulkResponse.Operations[0].Location);
        Assert.AreEqual((int)HttpStatusCode.Created, bulkResponse.Operations[0].Status);

        // Verify second operation processed with BulkId replaced
        Assert.AreEqual(Method.Put, bulkResponse.Operations[1].Method);
        Assert.AreEqual($"/Groups/{groupId}", bulkResponse.Operations[1].Path);
        Assert.AreEqual((int)HttpStatusCode.NoContent, bulkResponse.Operations[1].Status);

        await _service
            .Received(1)
            .UpdateAsync(
                Arg.Is<IContext>(c => AssertThatBulkIdWasReplaced(c, createdUserId)), 
                Arg.Any<CancellationToken>());
    }

    private bool AssertThatBulkIdWasReplaced(IContext context, Guid id)
    {
        var data = JsonConvert.DeserializeObject<JToken>((string)context.State.Resource!)!;
        var value = data.SelectToken("userReference")?.Value<string>();
        value.Should().Be(id.ToString());
        return true;
    }

    [TestMethod]
    public async Task ExecuteBulkOperationsAsync_ShouldThrowError_WhenBulkIdReferenceNotFound()
    {
        // Arrange
        _jsonSchemaProvider
            .ResolveJsonSchemaAsync(Arg.Any<IScimV2Context>(), "bulkJsonSchemaId")
            .Returns("{}");
        var bulkRequest = new BulkRequest
        {
            Operations = new List<BulkRequestOperation>
            {
                new BulkRequestOperation
                {
                    Method = Method.Post,
                    Path = "/Users",
                    Data = new JObject { { "name", "John Doe" } }
                },
                new BulkRequestOperation
                {
                    Method = Method.Put,
                    Path = $"/Groups/{Guid.NewGuid()}",
                    Data = new JObject { { "userReference", "bulkId:missingBulkId" } }
                }
            }
        };
        _context.State.Request = bulkRequest.ToJson();
        _context.Headers.Add("Ocp-Apim-Subscription-Key", "ocpApimSubscriptionKey");

        // Act & Assert
        await new BulkService(_serviceProvider, _contextFactory, _configuration, _jsonSchemaProvider)
                .ExecuteBulkOperationsAsync(_context, CancellationToken.None);

        var bulkResponse = JsonConvert.DeserializeObject<BulkResponse>((string)_context.Result!)!;
        Assert.AreEqual(2, bulkResponse.Operations.Count);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, bulkResponse.Operations[0].Status);
        var error = bulkResponse.Operations[1].Response!.ToObject<Error>()!;
        Assert.AreEqual(ErrorScimType.InvalidValue, error.ScimType);
        Assert.AreEqual("Bulk id missingBulkId not defined", error.Detail);
    }
}