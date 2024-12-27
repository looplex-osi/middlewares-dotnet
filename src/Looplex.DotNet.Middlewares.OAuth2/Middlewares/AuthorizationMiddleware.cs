using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Factories;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;
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

            if (string.IsNullOrEmpty(domain))
            {
                throw new HttpRequestException("Domain is required for authorization", null, HttpStatusCode.Unauthorized);
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new HttpRequestException("User ID is required for authorization", null, HttpStatusCode.Unauthorized);
            }

            // Extract resource from URL path
            string resource = GetResourceFromURL(context);

            // Convert HTTP method to RBAC action
            string action = ConvertHttpMethodToRbacAction(context);

            var rbacService = context.Services.GetRequiredService<IRbacService>();
            authorized = await rbacService.CheckPermissionAsync(userId, domain, resource, action);


            if (!authorized)
            {
                throw new HttpRequestException("No authorization to access this resource", null, HttpStatusCode.Unauthorized);
            }

            await next();
        });
        private static readonly Dictionary<string, string> HttpMethodToActionMap = new()
        {
            { "GET", "read" },
            { "HEAD", "read" },
            { "OPTIONS", "read" },
            { "POST", "write" },
            { "PUT", "write" },
            { "PATCH", "write" },
            { "DELETE", "delete" }
        };
        private static string ConvertHttpMethodToRbacAction(IContext context)
        {
            HttpContext httpContext = context.State.HttpContext;
            string method = httpContext.Request.Method?.ToUpperInvariant()
                ?? throw new ArgumentNullException("HTTP method is null");

            return HttpMethodToActionMap.TryGetValue(method, out var action)
                ? action
                : throw new NotSupportedException($"HTTP method '{method}' is not supported");
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

        private const string BearerPrefix = "Bearer ";

        private static string? GetUserIdFromToken(IContext context)
        {
            HttpContext httpContext = context.State.HttpContext;
            var configuration = context.Services.GetRequiredService<IConfiguration>();
            var audience = configuration["Audience"]
                    ?? throw new InvalidOperationException("Audience configuration is missing");
            var issuer = configuration["Issuer"]
                    ?? throw new InvalidOperationException("Issuer configuration is missing");

            string? authorization = httpContext.Request.Headers.Authorization;
            if (string.IsNullOrEmpty(authorization))
            {
                throw new HttpRequestException("Authorization header is missing", null, HttpStatusCode.Unauthorized);
            }
            if (!authorization.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpRequestException("Invalid authorization scheme", null, HttpStatusCode.Unauthorized);
            }

            string accessToken = authorization[BearerPrefix.Length..].Trim();
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new HttpRequestException("Access token is empty", null, HttpStatusCode.Unauthorized);
            }

            var jwtService = context.Services.GetRequiredService<IJwtService>();
            return jwtService.GetUserIdFromToken(accessToken);
        }
    }
}
