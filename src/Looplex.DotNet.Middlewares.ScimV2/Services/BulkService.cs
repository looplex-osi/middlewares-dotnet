using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Exceptions;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.OpenForExtension.Abstractions.Commands;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Looplex.OpenForExtension.Abstractions.ExtensionMethods;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Services;

public class BulkService(
    IServiceProvider serviceProvider,
    IContextFactory contextFactory) : IBulkService
{
    /// <summary>
    /// This is a collection with the default json.schemas that the application will use in its services.
    /// The action value of the json.schema is supposed to be resolved by an external service such as redis.
    /// </summary>
    internal static IList<string> SchemaIds = [];
    
    public async Task ExecuteBulkOperationsAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var serviceProviderConfiguration = serviceProvider.GetRequiredService<ServiceProviderConfiguration>();

        var json = context.GetRequiredValue<string>("Request");
        var bulkRequest = Resource.FromJson<BulkRequest>(json, out var messages);
        var bulkResponse = new BulkResponse();
        await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);

        if (messages.Count > 0)
            throw new EntityInvalidException(messages.ToList());

        ValidateBulkIdsUniqueness(bulkRequest);
        await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);

        context.Roles["BulkRequest"] = bulkRequest;
        context.Roles["BulkResponse"] = bulkResponse;
        await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

        if (!context.SkipDefaultAction)
        {
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

                    if (operation.Method == Domain.Entities.Messages.Method.Post)
                    {
                        await ExecutePostMethod(
                            context,
                            cancellationToken,
                            operationContext,
                            operation,
                            service,
                            bulkResponse,
                            resourceMap,
                            bulkIdCrossReference);
                    }
                    // TODO
                }
                catch (Exception e)
                {
                    errorCount++;
                    if (errorCount >= bulkRequest.FailOnErrors)
                    {
                        // TODO
                    }
                }
            }
            
            context.Result = bulkResponse.ToJson();;
        }

        await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
    }

    internal static async Task ExecutePostMethod(IContext context, CancellationToken cancellationToken,
        IContext operationContext, BulkRequestOperation operation, ICrudService service, BulkResponse bulkResponse,
        ResourceMap resourceMap, Dictionary<string, string> bulkIdCrossReference)
    {
        operationContext.State.Resource = JsonConvert.SerializeObject(operation.Data!);

        await service.CreateAsync(context, cancellationToken);
        var id = (string)context.Result!;

        bulkResponse.Operations.Add(new ()
        {
            Method = operation.Method,
            Path = operation.Path,
            Location = $"{resourceMap.Resource}/{id}",
            Status = (int)HttpStatusCode.Created
        });

        bulkIdCrossReference[operation.BulkId!] = id;
    }

    internal static void ValidateOperation(BulkRequestOperation operation)
    {
        if (operation.Data == null &&
            operation.Method != Domain.Entities.Messages.Method.Delete)
            throw new Error(
                $"Data should have value for method {operation.Method}",
                ErrorScimType.InvalidValue,
                HttpStatusCode.BadRequest.ToString());

        if (string.IsNullOrWhiteSpace(operation.BulkId) &&
            operation.Method == Domain.Entities.Messages.Method.Post)
            throw new Error(
                $"BulkId should have value for method {operation.Method}",
                ErrorScimType.InvalidValue,
                HttpStatusCode.BadRequest.ToString());
    }

    internal static void ValidateBulkIdsUniqueness(BulkRequest bulkRequest)
    {
        var nonUniqueBulkIds = bulkRequest.Operations
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
                HttpStatusCode.BadRequest.ToString());
        }
    }

    internal static (ResourceMap, Guid?) GetResourceMap(BulkRequestOperation operation,
        ServiceProviderConfiguration serviceProviderConfiguration)
    {
        var path = operation.Path;
        if (path.StartsWith("/"))
            path = path[1..];

        var indexOfSlash = path.IndexOf('/');

        if (indexOfSlash <= 0 && operation.Method != Domain.Entities.Messages.Method.Post)
            throw new Error(
                $"Path {operation.Path} should refer to a specific resource when method is {operation.Method}",
                ErrorScimType.InvalidPath,
                HttpStatusCode.BadRequest.ToString());

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
                    HttpStatusCode.BadRequest.ToString());
        }
        
        var resourceMap = serviceProviderConfiguration.Map
            .FirstOrDefault(rm => rm.Resource == resource);
                    
        if (resourceMap == null)
            throw new Error(
                $"Path {resource} does not exist",
                ErrorScimType.InvalidPath,
                HttpStatusCode.BadRequest.ToString());
        
        return (resourceMap, resourceUniqueId);
    }
}
