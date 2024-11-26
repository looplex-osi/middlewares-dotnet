namespace Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;

public interface IJsonSchemaProvider
{
    Task<List<string>> ResolveJsonSchemasAsync(string ocpApimSubscriptionKey, List<string> schemaIds, string? lang = null);
    Task<string?> ResolveJsonSchemaAsync(string ocpApimSubscriptionKey, string schemaId, string? lang);
}