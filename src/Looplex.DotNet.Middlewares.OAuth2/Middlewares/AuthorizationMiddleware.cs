using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Factories;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Looplex.DotNet.Middlewares.OAuth2.Middlewares
{
    public static partial class AuthorizationMiddleware
    {
        public static readonly MiddlewareDelegate AuthorizeMiddleware = new(async (context, cancellationToken, next) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            string? userId;
            userId = GetUserIdFromToken(context);

            string? domain = ((IScimV2Context)context).GetDomain();
            bool authorized = false;

            if (!string.IsNullOrEmpty(domain) && !string.IsNullOrEmpty(userId))
            {
                //Get resource 
                string resource = GetResourceFromURL(context);
                //Get action
                string action;
                action = ConvertHttpMethodToRbacAction(context);
                var rbacService = context.Services.GetRequiredService<IRbacService>();
                authorized = await rbacService.CheckPermissionAsync(userId, domain, resource, action);
            }

            if (!authorized)
            {
                throw new HttpRequestException("No authorization to access this resource", null, HttpStatusCode.Unauthorized);
            }

            await next();
        });

        private static string ConvertHttpMethodToRbacAction(IContext context)
        {
            HttpContext httpContext = context.State.HttpContext;
            string action;
            switch (httpContext.Request.Method)
            {
                case "POST":
                case "PUT":
                case "PATCH":
                    action = "write";
                    break;
                case "DELETE":
                    action = "delete";
                    break;
                case "GET":
                    action = "read";
                    break;
                default:
                    throw new Exception("HttpMethod not found");
            }

            return action;
        }

        private static string GetResourceFromURL(IContext context)
        {
            HttpContext httpContext = context.State.HttpContext;
            string[] route = httpContext.Request.Path.ToString().Split('/');
            string resource;
            if (route.Length > 1 &&
            (int.TryParse(route[route.Length - 1], out int temp) || Guid.TryParse(route[route.Length - 1], out Guid temGUID)))
            {
                resource = route[route.Length - 2] + "/" + route[route.Length - 1];
            }
            else
            {
                resource = route[route.Length - 1];
            }

            return resource;
        }

        private static string? GetUserIdFromToken(IContext context)
        {
            HttpContext httpContext = context.State.HttpContext;
            var configuration = context.Services.GetRequiredService<IConfiguration>()!;
            var audience = configuration["Audience"];
            var issuer = configuration["Issuer"];

            string accesToken = string.Empty;

            string? authorization = httpContext.Request.Headers.Authorization;

            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
            {
                accesToken = authorization["Bearer ".Length..].Trim();
            }
            var jwtService = context.Services.GetService<IJwtService>();

            string userId = jwtService!.GetUserIdFromToken(accesToken);

            return userId;
        }
    }
}
