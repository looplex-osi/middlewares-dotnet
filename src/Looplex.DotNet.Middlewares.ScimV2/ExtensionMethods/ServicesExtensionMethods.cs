using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;
using Looplex.DotNet.Middlewares.ScimV2.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public static class ServicesExtensionMethods
{
    public static void AddScimV2Services(this IServiceCollection services)
    {
        services.AddSingleton<ISchemaService, SchemaService>();
        services.AddSingleton<IResourceTypeService, ResourceTypeService>();

        var config = new ServiceProviderConfiguration
        {
            Patch = new()
            {
                Supported = true,
            },
            Bulk = new()
            {
                Supported = false,
            },
            Filter = new()
            {
                Supported = false,
            },
            ChangePassword = new()
            {
                Supported = false,
            },
            Sort = new()
            {
                Supported = false,
            },
            Etag = new()
            {
                Supported = false,
            },
            AuthenticationSchemes = new[]
            {
                new AuthenticationScheme
                {
                    Name = "Looplex Auth",
                    Description = "We follow the Open ID Connect (OIDC) Flow for authentication and OAuth 2.0 Token Exchange for authorization",
                    DocumentationUri = new Uri("https://learn.microsoft.com/en-us/entra/identity-platform/v2-protocols-oidc"),
                    SpecUri = new Uri("https://github.com/looplex-osi/middlewares-dotnet"),
                    Type = AuthenticationSchemeType.OAuth2
                }
            }
        };
        services.AddSingleton(config);
    }
}