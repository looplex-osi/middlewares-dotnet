using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.OpenForExtension.Abstractions.Commands;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Looplex.OpenForExtension.Abstractions.ExtensionMethods;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace Looplex.DotNet.Middlewares.ScimV2.Services;

public class SchemaService(
    IConfiguration configuration,
    ICacheService cacheService,
    IRestClient restClient) : ISchemaService
{
    /// <summary>
    /// This is a collection with the default json.schemas that the application will use in its services.
    /// The action value of the json.schema is supposed to be resolved by an external service such as redis.
    /// </summary>
    internal static IList<string> SchemaIds = [];
    
    public async Task GetAllAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);
        var page = context.GetRequiredValue<int>("Pagination.Page");
        var perPage = context.GetRequiredValue<int>("Pagination.PerPage");
        var lang = context.GetValue<string?>("Lang");
            
        await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

        if (!context.SkipDefaultAction)
        {
            var schemaIds = SchemaIds
                .Skip(PaginationUtils.GetOffset(perPage, page))
                .Take(perPage)
                .ToList();

            var records = await ResolveJsonSchemasAsync(schemaIds);
            
            var result = new PaginatedCollection
            {
                Records = records.Select(r => (object)r).ToList(),
                Page = page,
                PerPage = perPage,
                TotalCount = SchemaIds.Count
            };
            context.State.Pagination.TotalCount = schemaIds.Count;
            
            context.Result = JsonConvert.SerializeObject(result);
        }

        await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

        await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
    }

    private async Task<List<string>> ResolveJsonSchemasAsync(List<string> schemaIds, string? lang = null)
    {
        var result = new List<string>();

        foreach (var schemaId in schemaIds)
        {
            var jsonSchema = await ResolveJsonSchemaAsync(schemaId, lang);

            if (string.IsNullOrWhiteSpace(jsonSchema))
                throw new InvalidOperationException($"Json schema {lang} {schemaId} was not found.");
            
            result.Add(jsonSchema);
        }

        return result;
    }
    
    private Task<string?> ResolveJsonSchemaAsync(string schemaId, string? lang)
    {
        if (!string.IsNullOrWhiteSpace(lang))
            schemaId = schemaId.Replace("schema", lang);

        return ResolveJsonSchemaAsync(schemaId);
    }
    
    private async Task<string?> ResolveJsonSchemaAsync(string schemaId)
    {
        string? jsonSchema = null;
        if (await cacheService.TryGetCacheValueAsync(schemaId, out var value))
        {
            jsonSchema = value;
        }
        else
        {
            var jsonSchemaCodeUrl = configuration["JsonSchemaCodeUrl"]!;
            var request = new RestRequest($"{jsonSchemaCodeUrl}/{jsonSchema}", Method.Get);
            var response = await restClient.ExecuteAsync(request);
            jsonSchema = response.Content;
        }

        return jsonSchema;
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

        var schemaId = context.GetRequiredValue<string>("Id");
        var lang = context.GetValue<string?>("Lang");
        await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);

        if (!SchemaIds.Contains(schemaId))
            throw new InvalidOperationException($"{schemaId} does not exists or is not configured for this app.");
        var jsonSchema = await ResolveJsonSchemaAsync(schemaId, lang);

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

        var schemaId = context.GetRequiredValue<string>("Id");
        context.Plugins.Execute<IHandleInput>(context, cancellationToken);

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
