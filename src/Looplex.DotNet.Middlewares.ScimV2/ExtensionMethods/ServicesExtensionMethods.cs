using Looplex.DotNet.Middlewares.ScimV2.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods
{
    public static class ServicesExtensionMethods
    {
        public static void AddScimV2AutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ScimV2Profile));
        }

        public static void AddScimV2Services(this IServiceCollection services)
        {
            
        }
    }
}
