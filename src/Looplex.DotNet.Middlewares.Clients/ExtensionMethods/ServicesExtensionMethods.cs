using Looplex.DotNet.Middlewares.Clients.Entities.Clients;
using Looplex.DotNet.Middlewares.Clients.Profiles;
using Looplex.DotNet.Middlewares.ScimV2.Entities.Schemas;
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
    
    public static void AddClientsSchemas()
    {
        Schema.Add<Client>(File.ReadAllText("./Entities/Schemas/Client.1.0.schema.json"));
    }
}