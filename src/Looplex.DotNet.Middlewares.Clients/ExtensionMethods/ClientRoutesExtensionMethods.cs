using Looplex.DotNet.Middlewares.Clients.DTOs;
using Looplex.DotNet.Middlewares.Clients.Entities;
using Looplex.DotNet.Middlewares.OAuth2.Services;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Looplex.OpenForExtension.Plugins;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.Clients.ExtensionMethods
{
    public static class ClientRoutesExtensionMethods
    {
        public static void UseClientRoutes(this IEndpointRouteBuilder app, IList<IPlugin> plugins)
        {
            app.UseScimV2Routes<Client, ClientReadDTO, ClientWriteDTO, IClientService>(plugins);
        }
    }
}
