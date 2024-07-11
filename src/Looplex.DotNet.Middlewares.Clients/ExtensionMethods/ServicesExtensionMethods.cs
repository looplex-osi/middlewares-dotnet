using Looplex.DotNet.Middlewares.Clients.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.Clients.ExtensionMethods;

public static class ServicesExtensionMethods
{
    public static void AddClientsAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ClientsProfile));
    }

    public static void AddClientsServices(this IServiceCollection services)
    {
            
    }
}