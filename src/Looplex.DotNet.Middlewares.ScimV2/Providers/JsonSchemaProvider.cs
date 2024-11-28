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
    public async Task<List<string>> ResolveJsonSchemasAsync(string ocpApimSubscriptionKey, List<string> schemaIds, string? lang = null)
    {
        var result = new List<string>();

        foreach (var schemaId in schemaIds)
        {
            var jsonSchema = await ResolveJsonSchemaAsync(ocpApimSubscriptionKey, schemaId, lang);

            if (string.IsNullOrWhiteSpace(jsonSchema))
                throw new InvalidOperationException($"Json schema {lang} {schemaId} was not found.");
            
            result.Add(jsonSchema);
        }

        return result;
    }
    
    public async Task<string?> ResolveJsonSchemaAsync(string ocpApimSubscriptionKey, string schemaId, string? lang)
    {
        string? schema = null;
        if (!string.IsNullOrWhiteSpace(lang))
        {
            var localizedSchemaId = schemaId.Replace("schema", lang);
            schema = await ResolveJsonSchemaAsync(ocpApimSubscriptionKey, localizedSchemaId);
        }

        if (string.IsNullOrEmpty(schema))
            schema = await ResolveJsonSchemaAsync(ocpApimSubscriptionKey, schemaId);

        return schema;
    }
    
    private async Task<string?> ResolveJsonSchemaAsync(string ocpApimSubscriptionKey, string schemaId)
    {
        string? jsonSchema = await redisService.GetAsync(schemaId);
        if (string.IsNullOrEmpty(jsonSchema))
        {
            var jsonSchemaCodeUrl = configuration["JsonSchemaCodeUrl"]!;

            var request = new RestRequest($"{jsonSchemaCodeUrl}", Method.Get);
            request.AddQueryParameter("id", schemaId);
            request.AddHeader("Ocp-Apim-Subscription-Key", ocpApimSubscriptionKey);
            var response = await restClient.ExecuteAsync(request);
            jsonSchema = response.Content;
        }

        return jsonSchema;
    }
}