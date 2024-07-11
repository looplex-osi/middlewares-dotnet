﻿using Looplex.DotNet.Middlewares.Clients.DTOs;
using Looplex.DotNet.Middlewares.Clients.Entities;
using Looplex.DotNet.Middlewares.OAuth2.Services;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Microsoft.AspNetCore.Routing;

namespace Looplex.DotNet.Middlewares.Clients.ExtensionMethods
{
    public static class ClientRoutesExtensionMethods
    {
        public static void UseClientRoutes(this IEndpointRouteBuilder app, ScimV2RouteOptions? options = null)
        {
            app.UseScimV2Routes<Client, ClientReadDTO, ClientWriteDTO, IClientService>(options ?? new ScimV2RouteOptions());
        }
    }
}
