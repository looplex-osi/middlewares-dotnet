using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Looplex.DotNet.Middlewares.ScimV2.Providers;

public class JsonSchemaProvider(
    IConfiguration configuration,
    IRedisService redisService,
    IRestClient restClient) : IJsonSchemaProvider
{
    private const string JsonSchemaIgnoreWhenNotFoundKey = "JsonSchemaIgnoreWhenNotFound";
    private const string JsonSchemaCodeUrlKey = "JsonSchemaCodeUrl";
    private const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";

    public async Task<List<string?>> ResolveJsonSchemasAsync(string ocpApimSubscriptionKey, List<string> schemaIds, string? lang = null)
    {
        var result = new List<string?>();
        
        foreach (var schemaId in schemaIds)
        {
            var jsonSchema = await ResolveJsonSchemaAsync(ocpApimSubscriptionKey, schemaId, lang);
            
            result.Add(jsonSchema);
        }

        return result;
    }
    
    public async Task<string?> ResolveJsonSchemaAsync(string ocpApimSubscriptionKey, string schemaId, string? lang = null)
    {
        string? jsonSchema = null;
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