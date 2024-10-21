using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class ServicesExtensionMethods
{
    public static void AddScimV2Services(this IServiceCollection services)
    {
        services.AddSingleton<ISchemaService, SchemaService>();
    }
}