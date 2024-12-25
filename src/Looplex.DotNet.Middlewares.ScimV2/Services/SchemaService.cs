using System.Net;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;
using Looplex.OpenForExtension.Abstractions.Commands;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Looplex.OpenForExtension.Abstractions.ExtensionMethods;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Services;

public class SchemaService(IJsonSchemaProvider jsonSchemaProvider): ISchemaService
{
    /// <summary>
    /// This is a collection with the default json.schemas that the application will use in its services.
    /// The action value of the json.schema is supposed to be resolved by an external service such as redis.
    /// </summary>
    internal static IList<string> SchemaIds = [];
    
    public async Task GetAllAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var startIndex = context.GetRequiredValue<int>("Pagination.StartIndex");
        var itemsPerPage = context.GetRequiredValue<int>("Pagination.ItemsPerPage");
        var lang = context.GetHeader("Lang");
        await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

        if (!context.SkipDefaultAction)
        {
            var schemaIds = SchemaIds
                .Skip(Math.Min(0, startIndex - 1))
                .Take(itemsPerPage)
                .ToList();

            var records = await jsonSchemaProvider.ResolveJsonSchemasAsync(context, schemaIds, lang);
            
            var result = new ListResponse
            {
                Resources = records.Where(r => r != null).Select(r => (object)r!).ToList(),
                StartIndex = startIndex,
                ItemsPerPage = itemsPerPage,
                TotalResults = SchemaIds.Count
            };
            context.State.Pagination.TotalCount = SchemaIds.Count;
            
            context.Result = JsonConvert.SerializeObject(result);
        }

        await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
    }

    /// <summary>
    /// Given a schemaId (must be present in SchemasIds), resolves the json schema
    /// from cache service or resolve url.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task GetByIdAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var schemaId = context.GetRequiredRouteValue<string>("schemaId");
        var lang = context.GetHeader("Lang");
        await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);

        if (!SchemaIds.Contains(schemaId))
            throw new InvalidOperationException($"{schemaId} does not exists or is not configured for this app.");
        var jsonSchema = await jsonSchemaProvider.ResolveJsonSchemaAsync(context, schemaId, lang);

        if (string.IsNullOrWhiteSpace(jsonSchema))
            throw new InvalidOperationException($"Json schema {lang} {schemaId} was not found.");
        await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);
        
        context.Roles.Add("JsonSchema", jsonSchema);
        await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

        if (!context.SkipDefaultAction)
        {
            context.Result = (string)context.Roles["JsonSchema"];
        }

        await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
    }
    
    /// <summary>
    /// This adds an schemaId to the SchemaIds in memory list.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task CreateAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var schemaId = context.GetRequiredRouteValue<string>("schemaId");
        context.Plugins.Execute<IHandleInput>(context, cancellationToken);

        if (string.IsNullOrWhiteSpace(schemaId))
            throw new Error($"SchemaId cannot be empty", (int)HttpStatusCode.BadRequest);
        context.Plugins.Execute<IValidateInput>(context, cancellationToken);

        context.Roles.Add("SchemaId", schemaId);
        context.Plugins.Execute<IDefineRoles>(context, cancellationToken);

        context.Plugins.Execute<IBind>(context, cancellationToken);

        context.Plugins.Execute<IBeforeAction>(context, cancellationToken);

        if (!context.SkipDefaultAction)
        {
            SchemaIds.Add(context.Roles["SchemaId"]);
        }

        context.Plugins.Execute<IAfterAction>(context, cancellationToken);

        context.Plugins.Execute<IReleaseUnmanagedResources>(context, cancellationToken);

        return Task.CompletedTask;
    }
}
