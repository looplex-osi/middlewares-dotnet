using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Looplex.DotNet.Middlewares.ScimV2.Providers;

public class JsonSchemaProvider(
    IConfiguration configuration,
    ICacheService cacheService,
    IRestClient restClient) : IJsonSchemaProvider
{
    public async Task<List<string>> ResolveJsonSchemasAsync(List<string> schemaIds, string? lang = null)
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
    
    public async Task<string?> ResolveJsonSchemaAsync(string schemaId, string? lang)
    {
        string? schema = null;
        if (!string.IsNullOrWhiteSpace(lang))
        {
            var localizedSchemaId = schemaId.Replace("schema", lang);
            schema = await ResolveJsonSchemaAsync(localizedSchemaId);
        }

        if (string.IsNullOrEmpty(schema))
            schema = await ResolveJsonSchemaAsync(schemaId);

        return schema;
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
            var jsonSchemaCodeApiKey = configuration["JsonSchemaCodeApiKey"]!;

            var request = new RestRequest($"{jsonSchemaCodeUrl}/{jsonSchema}", Method.Get);
            request.AddHeader("Ocp-Apim-Subscription-Key", jsonSchemaCodeApiKey);
            var response = await restClient.ExecuteAsync(request);
            jsonSchema = response.Content;
        }

        return jsonSchema;
    }
}