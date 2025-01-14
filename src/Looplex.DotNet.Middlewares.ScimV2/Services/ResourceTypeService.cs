using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.OpenForExtensions;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Services;

public class ResourceTypeService(
    IExtensionPointOrchestrator extensionPointOrchestrator) : IResourceTypeService
{
    internal static IList<ResourceType> ResourceTypes = [];
    
    #region GetAll
    
    public Task GetAllAsync(IContext context, CancellationToken cancellationToken)
    {
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

    private readonly ExtensionPointAsyncDelegate _getAllHandleInputAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getAllValidateInputAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getAllDefineRolesAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getAllBindAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getAllBeforeActionAsync = _ => Task.CompletedTask;

    private readonly ExtensionPointAsyncDelegate _getAllDefaultActionAsync = async context =>
    {
        var startIndex = context.GetRequiredValue<int>("Pagination.StartIndex");
        var itemsPerPage = context.GetRequiredValue<int>("Pagination.ItemsPerPage");
        
        var records = ResourceTypes
            .Skip(Math.Min(0, startIndex - 1))
            .Take(itemsPerPage)
            .ToList();
            
        var result = new ListResponse
        {
            Resources = records.Select(r => (object)r).ToList(),
            StartIndex = startIndex,
            ItemsPerPage = itemsPerPage,
            TotalResults = ResourceTypes.Count
        };
        context.State.Pagination.TotalCount = ResourceTypes.Count;
            
        context.Result = JsonConvert.SerializeObject(result);
    };
    
    private readonly ExtensionPointAsyncDelegate _getAllAfterActionAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getAllReleaseUnmanagedResourcesAsync = _ => Task.CompletedTask;
    
    #endregion
    
    #region GetById
    
    public Task GetByIdAsync(IContext context, CancellationToken cancellationToken)
    {
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            _getByIdHandleInputAsync,
            _getByIdValidateInputAsync,
            _getByIdDefineRolesAsync,
            _getByIdBindAsync,
            _getByIdBeforeActionAsync,
            _getByIdDefaultActionAsync,
            _getByIdAfterActionAsync,
            _getByIdReleaseUnmanagedResourcesAsync,
            cancellationToken);
    }

    private readonly ExtensionPointAsyncDelegate _getByIdHandleInputAsync = context =>
    {
        var id = context.GetRequiredRouteValue<string>("resourceTypeId");
        context.State.ResourceTypeId = id;
        return Task.CompletedTask;
    };
    
    private readonly ExtensionPointAsyncDelegate _getByIdValidateInputAsync = context =>
    {
        var id = context.GetRequiredValue<string>("ResourceTypeId");
        var resourceType = ResourceTypes.FirstOrDefault(rt => rt.Id == id);
        if (resourceType == default)
            throw new InvalidOperationException($"{id} does not exists or is not configured for this app.");
        context.State.ResourceType = resourceType;
        return Task.CompletedTask;
    };
    
    private readonly ExtensionPointAsyncDelegate _getByIdDefineRolesAsync = context =>
    {
        var resourceType = context.GetRequiredValue<ResourceType>("ResourceType");
        context.Roles.Add("ResourceType", resourceType);
        return Task.CompletedTask;
    };
    
    private readonly ExtensionPointAsyncDelegate _getByIdBindAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getByIdBeforeActionAsync = _ => Task.CompletedTask;

    private readonly ExtensionPointAsyncDelegate _getByIdDefaultActionAsync = context =>
    {
        context.Result = ((ResourceType)context.Roles["ResourceType"]).ToJson();
        return Task.CompletedTask;
    };
    
    private readonly ExtensionPointAsyncDelegate _getByIdAfterActionAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getByIdReleaseUnmanagedResourcesAsync = _ => Task.CompletedTask;
    
    #endregion

    #region Create
    
    public Task CreateAsync(IContext context, CancellationToken cancellationToken)
    {
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            _createHandleInputAsync,
            _createValidateInputAsync,
            _createDefineRolesAsync,
            _createBindAsync,
            _createBeforeActionAsync,
            _createDefaultActionAsync,
            _createAfterActionAsync,
            _createReleaseUnmanagedResourcesAsync,
            cancellationToken);
    }

    private readonly ExtensionPointAsyncDelegate _createHandleInputAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _createValidateInputAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _createDefineRolesAsync = context =>
    {
        var resourceType = context.GetRequiredValue<ResourceType>("ResourceType");
        context.Roles.Add("ResourceType", resourceType);
        return Task.CompletedTask;
    };
    
    private readonly ExtensionPointAsyncDelegate _createBindAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _createBeforeActionAsync = _ => Task.CompletedTask;

    private readonly ExtensionPointAsyncDelegate _createDefaultActionAsync = context =>
    {
        ResourceTypes.Add(context.Roles["ResourceType"]);
        return Task.CompletedTask;
    };
    
    private readonly ExtensionPointAsyncDelegate _createAfterActionAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _createReleaseUnmanagedResourcesAsync = _ => Task.CompletedTask;
    
    #endregion
}
