using System.Net;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.OpenForExtensions;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Services;

public class SchemaService(
    IExtensionPointOrchestrator extensionPointOrchestrator,
    IJsonSchemaProvider jsonSchemaProvider): ISchemaService
{
    /// <summary>
    /// This is a collection with the default json.schemas that the application will use in its services.
    /// The action value of the json.schema is supposed to be resolved by an external service such as redis.
    /// </summary>
    internal static IList<string> SchemaIds = [];
    
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

    private readonly ExtensionPointAsyncDelegate _getAllHandleInputAsync = context =>
    {
        var lang = context.GetHeader("Lang");
        if (!string.IsNullOrWhiteSpace(lang))
            context.State.Lang = lang;
        return Task.CompletedTask;
    };

    private readonly ExtensionPointAsyncDelegate _getAllValidateInputAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getAllDefineRolesAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getAllBindAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getAllBeforeActionAsync = _ => Task.CompletedTask;

    private readonly ExtensionPointAsyncDelegate _getAllDefaultActionAsync = async context =>
    {
        var startIndex = context.GetRequiredValue<int>("Pagination.StartIndex");
        var itemsPerPage = context.GetRequiredValue<int>("Pagination.ItemsPerPage");
        var lang = context.GetValue<string>("Lang");
        
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
    };
    
    private readonly ExtensionPointAsyncDelegate _getAllAfterActionAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getAllReleaseUnmanagedResourcesAsync = _ => Task.CompletedTask;
    
    #endregion
    
    #region GetById
    
    /// <summary>
    /// Given a schemaId (must be present in SchemasIds), resolves the json schema
    /// from cache service or resolve url.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
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
        var lang = context.GetHeader("Lang");
        if (!string.IsNullOrWhiteSpace(lang))
            context.State.Lang = lang;
        return Task.CompletedTask;
    };

    private readonly ExtensionPointAsyncDelegate _getByIdValidateInputAsync = async context =>
    {
        var schemaId = context.GetRequiredRouteValue<string>("schemaId");
        var lang = context.GetValue<string>("Lang");

        if (!SchemaIds.Contains(schemaId))
            throw new InvalidOperationException($"{schemaId} does not exists or is not configured for this app.");
        var jsonSchema = await jsonSchemaProvider.ResolveJsonSchemaAsync(context, schemaId, lang);

        if (string.IsNullOrWhiteSpace(jsonSchema))
            throw new InvalidOperationException($"Json schema {lang} {schemaId} was not found.");

        context.State.JsonSchema = jsonSchema;
    };

    private readonly ExtensionPointAsyncDelegate _getByIdDefineRolesAsync = context =>
    {
        var jsonSchema = context.GetRequiredValue<string>("JsonSchema");
        context.Roles.Add("JsonSchema", jsonSchema);
        return Task.CompletedTask;
    };
        
    private readonly ExtensionPointAsyncDelegate _getByIdBindAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getByIdBeforeActionAsync = _ => Task.CompletedTask;

    private readonly ExtensionPointAsyncDelegate _getByIdDefaultActionAsync = context =>
    {
        context.Result = (string)context.Roles["JsonSchema"];
        return Task.CompletedTask;
    };
    
    private readonly ExtensionPointAsyncDelegate _getByIdAfterActionAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _getByIdReleaseUnmanagedResourcesAsync = _ => Task.CompletedTask;
    
    #endregion
    
    #region Create
    
    /// <summary>
    /// This adds an schemaId to the SchemaIds in memory list.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    private readonly ExtensionPointAsyncDelegate _createValidateInputAsync = context =>
    {
        var schemaId = context.GetRequiredRouteValue<string>("schemaId");

        if (string.IsNullOrWhiteSpace(schemaId))
            throw new Error($"SchemaId cannot be empty", (int)HttpStatusCode.BadRequest);

        context.State.SchemaId = schemaId;

        return Task.CompletedTask;
    };

    private readonly ExtensionPointAsyncDelegate _createDefineRolesAsync = context =>
    {
        var jsonSchema = context.GetRequiredValue<string>("SchemaId");
        context.Roles.Add("SchemaId", jsonSchema);
        return Task.CompletedTask;
    };
        
    private readonly ExtensionPointAsyncDelegate _createBindAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _createBeforeActionAsync = _ => Task.CompletedTask;

    private readonly ExtensionPointAsyncDelegate _createDefaultActionAsync = context =>
    {
        SchemaIds.Add(context.Roles["SchemaId"]);
        return Task.CompletedTask;
    };
    
    private readonly ExtensionPointAsyncDelegate _createAfterActionAsync = _ => Task.CompletedTask;
    private readonly ExtensionPointAsyncDelegate _createReleaseUnmanagedResourcesAsync = _ => Task.CompletedTask;
    
    #endregion
}
