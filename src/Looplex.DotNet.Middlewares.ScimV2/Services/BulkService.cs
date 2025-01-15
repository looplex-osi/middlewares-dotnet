using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.OpenForExtensions;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Looplex.DotNet.Middlewares.ScimV2.Services;

public class BulkService(
    IRbacService rbacService,
    IExtensionPointOrchestrator extensionPointOrchestrator,
    IServiceProvider serviceProvider,
    IContextFactory contextFactory,
    IConfiguration configuration,
    IJsonSchemaProvider jsonSchemaProvider) : IBulkService
{
    const string BulkIdValuePrefix = "bulkId:";
    
    #region ExecuteBulkOperations
    
    public Task ExecuteBulkOperationsAsync(IContext context, CancellationToken cancellationToken)
    {
        rbacService.ThrowIfUnauthorized(context, GetType().Name, this.GetCallerName(), cancellationToken);

        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            _getAllHandleInputAsync,
            _getAllValidateInputAsync,
            _getAllDefineRolesAsync,
            _getAllBindAsync,
            _getAllBeforeActionAsync,
            _getAllDefaultActionAsync,
            _getAllAfterActionAsync,
            _getAllReleaseUnmanagedResourcesAsync,
            cancellationToken);
    }

    private readonly ExtensionPointAsyncDelegate _getAllHandleInputAsync = async (context, _) =>
    {
        var schemaId = configuration["JsonSchemaIdForBulkOperation"]!;
        var jsonSchema = await jsonSchemaProvider.ResolveJsonSchemaAsync(context, schemaId);
        
        var json = context.GetRequiredValue<string>("Request");
        var bulkRequest = Resource.FromJson<BulkRequest>(json, jsonSchema, out var messages);
        context.State.Messages = messages;
        context.State.BulkRequest = bulkRequest;
    };
    
    private readonly ExtensionPointAsyncDelegate _getAllValidateInputAsync = (context, _) =>
    {
        var messages = context.GetRequiredValue<IList<string>>("Messages");
        var bulkRequest = context.GetRequiredValue<BulkRequest>("BulkRequest");
        if (messages.Count > 0)
            throw new EntityInvalidException(messages.ToList());

        ValidateBulkIdsUniqueness(bulkRequest);
        return Task.CompletedTask;
    };
    private readonly ExtensionPointAsyncDelegate _getAllDefineRolesAsync = (context, _) =>
    {
        var bulkRequest = context.GetRequiredValue<BulkRequest>("BulkRequest");
        var bulkResponse = new BulkResponse();
        context.Roles["BulkRequest"] = bulkRequest;
        context.Roles["BulkResponse"] = bulkResponse;
        return Task.CompletedTask;
    };
    private readonly ExtensionPointAsyncDelegate _getAllBindAsync = (_, _) => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getAllBeforeActionAsync = (_, _) => Task.CompletedTask;

    private readonly ExtensionPointAsyncDelegate _getAllDefaultActionAsync = async (context, cancellationToken) =>
    {
        var serviceProviderConfiguration = serviceProvider.GetRequiredService<ServiceProviderConfiguration>();
        var bulkRequest = (BulkRequest)context.Roles["BulkRequest"];
        var bulkResponse = (BulkResponse)context.Roles["BulkResponse"];
        
        var errorCount = 0;
            Dictionary<string, string> bulkIdCrossReference = [];
            
            foreach (var operation in bulkRequest.Operations)
            {
                try
                {
                    ValidateOperation(operation);

                    var (resourceMap, resourceUniqueId) = GetResourceMap(operation, serviceProviderConfiguration);

                    var service = (ICrudService)serviceProvider.GetRequiredService(resourceMap.Service);
                    var operationContext = contextFactory.Create([]);
                    operationContext.State.ParentContext = context;
                    
                    if (operation.Data != null)
                    {
                        JsonUtils.Traverse(operation.Data, BulkIdVisitor(bulkIdCrossReference));
                    }
                    
                    if (operation.Method == Method.Post)
                    {
                        var id = await ExecutePostMethod(
                            operationContext,
                            operation,
                            service,
                            bulkResponse,
                            resourceMap,
                            cancellationToken);

                        bulkIdCrossReference[operation.BulkId!] = id;
                    }
                    else if (operation.Method == Method.Put)
                    {
                        await ExecutePutMethod(
                            operationContext,
                            operation,
                            service,
                            bulkResponse,
                            resourceMap,
                            resourceUniqueId!.Value,
                            cancellationToken);
                    }
                    else if (operation.Method == Method.Put)
                    {
                        await ExecutePatchMethod(
                            operationContext,
                            operation,
                            service,
                            bulkResponse,
                            resourceMap,
                            resourceUniqueId!.Value,
                            cancellationToken);
                    }
                    else if (operation.Method == Method.Delete)
                    {
                        await ExecuteDeleteMethod(
                            operationContext,
                            operation,
                            service,
                            bulkResponse,
                            resourceUniqueId!.Value,
                            cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    if (e is not Error error)
                        error = new Error(
                            e.Message,
                            (int)HttpStatusCode.InternalServerError);
                    
                    errorCount++;
                    if (errorCount > bulkRequest.FailOnErrors)
                        break;
                    bulkResponse.Operations.Add(new ()
                    {
                        Method = operation.Method,
                        Path = operation.Path,
                        Status = error.Status,
                        Response = error.ToJToken()
                    });
                }
            }
            
            context.Result = bulkResponse.ToJson();
    };
    
    private readonly ExtensionPointAsyncDelegate _getAllAfterActionAsync = (_, _) => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getAllReleaseUnmanagedResourcesAsync = (_, _) => Task.CompletedTask;
    
    #endregion

    internal static Action<JToken> BulkIdVisitor(Dictionary<string, string> bulkIdCrossReference)
    {
        return (node) =>
        {
            if (node.Type == JTokenType.String)
            {
                var nodeValue = node.Value<string>();

                if (!string.IsNullOrWhiteSpace(nodeValue) && nodeValue.StartsWith(BulkIdValuePrefix))
                {
                    var key = nodeValue[BulkIdValuePrefix.Length..];

                    if (!bulkIdCrossReference.TryGetValue(key, out var bulkIdValue))
                        throw new Error(
                            $"Bulk id {key} not defined",
                            ErrorScimType.InvalidValue,
                            (int)HttpStatusCode.BadRequest);
                                    
                    node.Replace(bulkIdValue);
                }
            }
        };
    }

    internal static async Task<string> ExecutePostMethod(
        IContext operationContext, BulkRequestOperation operation, ICrudService service, BulkResponse bulkResponse,
        ResourceMap resourceMap, CancellationToken cancellationToken)
    {
        operationContext.State.Resource = JsonConvert.SerializeObject(operation.Data!);

        await service.CreateAsync(operationContext, cancellationToken);
        var id = (string)operationContext.Result!;

        bulkResponse.Operations.Add(new ()
        {
            Method = operation.Method,
            Path = operation.Path,
            Location = $"{resourceMap.Resource}/{id}",
            Status = (int)HttpStatusCode.Created
        });

        return id;
    }
    
    internal static async Task ExecutePutMethod(
        IContext operationContext, BulkRequestOperation operation, ICrudService service, BulkResponse bulkResponse,
        ResourceMap resourceMap, Guid resourceUniqueId, CancellationToken cancellationToken)
    {
        operationContext.State.Id = resourceUniqueId.ToString();
        operationContext.State.Resource = JsonConvert.SerializeObject(operation.Data!);

        await service.UpdateAsync(operationContext, cancellationToken);
        var id = (string)operationContext.Result!;

        bulkResponse.Operations.Add(new ()
        {
            Method = operation.Method,
            Path = operation.Path,
            Location = $"{resourceMap.Resource}/{id}",
            Status = (int)HttpStatusCode.NoContent
        });
    }
    
    internal static async Task ExecutePatchMethod(
        IContext operationContext, BulkRequestOperation operation, ICrudService service, BulkResponse bulkResponse,
        ResourceMap resourceMap, Guid resourceUniqueId, CancellationToken cancellationToken)
    {
        operationContext.State.Id = resourceUniqueId.ToString();
        operationContext.State.Operations = JsonConvert.SerializeObject(operation.Data!);

        await service.UpdateAsync(operationContext, cancellationToken);
        var id = (string)operationContext.Result!;

        bulkResponse.Operations.Add(new ()
        {
            Method = operation.Method,
            Path = operation.Path,
            Location = $"{resourceMap.Resource}/{id}",
            Status = (int)HttpStatusCode.NoContent
        });
    }
    
    internal static async Task ExecuteDeleteMethod(
        IContext operationContext, BulkRequestOperation operation, ICrudService service, BulkResponse bulkResponse,
        Guid resourceUniqueId, CancellationToken cancellationToken)
    {
        operationContext.State.Id = resourceUniqueId.ToString();

        await service.DeleteAsync(operationContext, cancellationToken);

        bulkResponse.Operations.Add(new ()
        {
            Method = operation.Method,
            Path = operation.Path,
            Status = (int)HttpStatusCode.NoContent
        });
    }

    internal static void ValidateOperation(BulkRequestOperation operation)
    {
        if (operation.Data == null &&
            operation.Method != Method.Delete)
            throw new Error(
                $"Data should have value for method {operation.Method}",
                ErrorScimType.InvalidValue,
                (int)HttpStatusCode.BadRequest);

        if (string.IsNullOrWhiteSpace(operation.BulkId) &&
            operation.Method == Method.Post)
            throw new Error(
                $"BulkId should have value for method {operation.Method}",
                ErrorScimType.InvalidValue,
                (int)HttpStatusCode.BadRequest);
    }

    internal static void ValidateBulkIdsUniqueness(BulkRequest bulkRequest)
    {
        var nonUniqueBulkIds = bulkRequest.Operations
            .Where(o => !string.IsNullOrWhiteSpace(o.BulkId))
            .GroupBy(o => o.BulkId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (nonUniqueBulkIds.Any())
        {
            var bulkIds = string.Join(", ", nonUniqueBulkIds);
            throw new Error(
                $"BulkIds {bulkIds} must be unique",
                ErrorScimType.Uniqueness,
                (int)HttpStatusCode.BadRequest);
        }
    }

    internal static (ResourceMap, Guid?) GetResourceMap(BulkRequestOperation operation,
        ServiceProviderConfiguration serviceProviderConfiguration)
    {
        var path = operation.Path;
        if (path.StartsWith("/"))
            path = path[1..];

        var indexOfSlash = path.IndexOf('/');

        if (indexOfSlash <= 0 && operation.Method != Method.Post)
            throw new Error(
                $"Path {operation.Path} should refer to a specific resource when method is {operation.Method}",
                ErrorScimType.InvalidPath,
                (int)HttpStatusCode.BadRequest);

        var resource = path;
        Guid? resourceUniqueId = null;
        
        if (indexOfSlash > 0 && indexOfSlash < path.Length)
        {
            resource = path[..indexOfSlash];
            var resourceIdentifier = path[(indexOfSlash + 1)..];

            if (Guid.TryParse(resourceIdentifier, out var uuid))
            {
                resourceUniqueId = uuid;
            }
            else
                throw new Error(
                    $"Resource identifier {resourceIdentifier} is not valid",
                    ErrorScimType.InvalidValue,
                    (int)HttpStatusCode.BadRequest);
        }
        
        var resourceMap = serviceProviderConfiguration.Map
            .FirstOrDefault(rm => rm.Resource == resource);
                    
        if (resourceMap == null)
            throw new Error(
                $"Path {resource} does not exist",
                ErrorScimType.InvalidPath,
                (int)HttpStatusCode.BadRequest);
        
        return (resourceMap, resourceUniqueId);
    }
}
