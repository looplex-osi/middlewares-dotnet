using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.Extensions.Configuration;
using RestSharp;
using Method = RestSharp.Method;

namespace Looplex.DotNet.Middlewares.ScimV2.Providers;

public class JsonSchemaProvider(
    IConfiguration configuration,
    IRedisService redisService,
    IRestClient restClient) : IJsonSchemaProvider
{
    private const string JsonSchemaIgnoreWhenNotFoundKey = "JsonSchemaIgnoreWhenNotFound";
    private const string JsonSchemaCodeUrlKey = "JsonSchemaCodeUrl";
    private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

    public async Task<List<string?>> ResolveJsonSchemasAsync(IContext context, List<string> schemaIds, string? lang = null)
    {
        var result = new List<string?>();
        
        foreach (var schemaId in schemaIds)
        {
            var jsonSchema = await ResolveJsonSchemaAsync(context, schemaId, lang);
            
            result.Add(jsonSchema);
        }

        return result;
    }
    
    public async Task<string?> ResolveJsonSchemaAsync(IContext context, string schemaId, string? lang = null)
    {
        var scimContext = context as IScimV2Context;
        if (scimContext == null)
            throw new ArgumentException("context is not a scim v2 context", nameof(context));
        
        string? jsonSchema = null;
        
        if (!(scimContext).Headers.TryGetValue(OcpApimSubscriptionKeyHeader, out var ocpApimSubscriptionKey))
            throw new Error($"Missing header {OcpApimSubscriptionKeyHeader} in request.", (int)HttpStatusCode.Forbidden);
            
        var jsonSchemaIgnoreWhenNotFound = configuration.GetValue<bool>(JsonSchemaIgnoreWhenNotFoundKey);
        
        if (!string.IsNullOrWhiteSpace(lang))
        {
            var localizedSchemaId = schemaId.Replace("schema", lang);
            jsonSchema = await ResolveLocalizedJsonSchemaAsync(ocpApimSubscriptionKey, localizedSchemaId);
        }

        if (string.IsNullOrEmpty(jsonSchema))
            jsonSchema = await ResolveLocalizedJsonSchemaAsync(ocpApimSubscriptionKey, schemaId);

        if (string.IsNullOrWhiteSpace(jsonSchema))
        {
            if (jsonSchemaIgnoreWhenNotFound)
                jsonSchema = "{}";
            else
                throw new InvalidOperationException($"Json schema {schemaId} was not found.");
        }
        
        return jsonSchema;
    }
    
    private async Task<string?> ResolveLocalizedJsonSchemaAsync(string ocpApimSubscriptionKey, string schemaId)
    {
        string? jsonSchema = await redisService.GetAsync(schemaId);
        if (string.IsNullOrEmpty(jsonSchema))
        {
            var jsonSchemaCodeUrl = configuration[JsonSchemaCodeUrlKey]!;

            var request = new RestRequest($"{jsonSchemaCodeUrl}", Method.Get);
            request.AddQueryParameter("id", schemaId);
            request.AddHeader(OcpApimSubscriptionKeyHeader, ocpApimSubscriptionKey);
            var response = await restClient.ExecuteAsync(request);
            jsonSchema = response.Content;
        }

        return jsonSchema;
    }
}