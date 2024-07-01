using Looplex.DotNet.Middlewares.OAuth2.Services;
using Looplex.DotNet.Middlewares.OAuth2.Storages.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.OAuth2.Storages.Default.ExtensionMethods
{
    public static class ServicesExtensionMethods
    {
        public static void AddOAuth2InMemoryServices(this IServiceCollection services)
        {
            services.AddSingleton<IClientCredentialService, ClientCredentialService>();
        }

        public static void AddOAuth2InMemoryAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(OAuth2InMemoryProfile));
        }
    }
}
