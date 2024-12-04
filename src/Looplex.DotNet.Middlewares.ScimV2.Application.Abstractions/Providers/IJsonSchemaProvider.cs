using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Providers;

public interface IJsonSchemaProvider
{
    Task<List<string?>> ResolveJsonSchemasAsync(IContext context, List<string> schemaIds, string? lang = null);
    Task<string?> ResolveJsonSchemaAsync(IContext context, string schemaId, string? lang = null);
}