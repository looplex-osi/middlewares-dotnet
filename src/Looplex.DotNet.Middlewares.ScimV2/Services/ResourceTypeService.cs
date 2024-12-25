using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;
using Looplex.OpenForExtension.Abstractions.Commands;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Looplex.OpenForExtension.Abstractions.ExtensionMethods;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Services;

public class ResourceTypeService() : IResourceTypeService
{
    internal static IList<ResourceType> ResourceTypes = [];
    
    public async Task GetAllAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);
        var startIndex = context.GetRequiredValue<int>("Pagination.StartIndex");
        var itemsPerPage = context.GetRequiredValue<int>("Pagination.ItemsPerPage");
            
        await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

        if (!context.SkipDefaultAction)
        {
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
        }

        await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
    }
    
    public async Task GetByIdAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = context.GetRequiredRouteValue<string>("resourceTypeId");
        await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);

        var resourceType = ResourceTypes.FirstOrDefault(rt => rt.Id == id);
        if (resourceType == default)
            throw new InvalidOperationException($"{id} does not exists or is not configured for this app.");
        await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);
        
        context.Roles.Add("ResourceType", resourceType);
        await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

        if (!context.SkipDefaultAction)
        {
            context.Result = ((ResourceType)context.Roles["ResourceType"]).ToJson();
        }

        await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
    }

    public Task CreateAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var resourceType = context.GetRequiredValue<ResourceType>("ResourceType");
        context.Plugins.Execute<IHandleInput>(context, cancellationToken);

        context.Plugins.Execute<IValidateInput>(context, cancellationToken);

        context.Roles.Add("ResourceType", resourceType);
        context.Plugins.Execute<IDefineRoles>(context, cancellationToken);

        context.Plugins.Execute<IBind>(context, cancellationToken);

        context.Plugins.Execute<IBeforeAction>(context, cancellationToken);

        if (!context.SkipDefaultAction)
        {
            ResourceTypes.Add(context.Roles["ResourceType"]);
        }

        context.Plugins.Execute<IAfterAction>(context, cancellationToken);

        context.Plugins.Execute<IReleaseUnmanagedResources>(context, cancellationToken);

        return Task.CompletedTask;
    }
}
