namespace Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;

public interface IJsonSchemaProvider
{
    Task<List<string>> ResolveJsonSchemasAsync(List<string> schemaIds, string? lang = null);
    Task<string?> ResolveJsonSchemaAsync(string schemaId, string? lang);
}