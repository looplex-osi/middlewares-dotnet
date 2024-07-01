using Looplex.DotNet.Middlewares.OAuth2.Profiles;
using Looplex.DotNet.Middlewares.OAuth2.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods
{
    public static class ServicesExtensionMethods
    {
        public static void AddOAuth2AutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(OAuth2Profile));
        }

        public static void AddOAuth2Services(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationService, AuthorizationService>();
            services.AddSingleton<IIdTokenService, IdTokenService>();
        }
    }
}
