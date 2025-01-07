using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Looplex.DotNet.Middlewares.OAuth2.Middlewares;

public static partial class OAuth2Middlewares
{
    public static readonly MiddlewareDelegate AuthorizeMiddleware = async (context, cancellationToken, next) =>
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userId = GetUserIdFromToken(context);

        var domain = ((IScimV2Context)context).GetDomain();

        if (string.IsNullOrEmpty(domain))
        {
            throw new HttpRequestException("Domain is required for authorization", null,
                HttpStatusCode.Unauthorized);
        }

        if (string.IsNullOrEmpty(userId))
        {
            throw new HttpRequestException("User ID is required for authorization", null,
                HttpStatusCode.Unauthorized);
        }

        // Extract resource from URL path
        var resource = GetResourceFromUrl(context);

        // Convert HTTP method to RBAC action
        var action = ConvertHttpMethodToRbacAction(context);

        var rbacService = context.Services.GetRequiredService<IRbacService>();
        var authorized = await rbacService.CheckPermissionAsync(userId, domain, resource, action);


        if (!authorized)
        {
            throw new HttpRequestException("No authorization to access this resource", null,
                HttpStatusCode.Unauthorized);
        }

        await next();
    };

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
        if (string.IsNullOrWhiteSpace(httpContext.Request.Method))
            throw new ArgumentNullException(nameof(context), "HTTP method is null");
        var method = httpContext.Request.Method.ToUpperInvariant();

        return HttpMethodToActionMap.TryGetValue(method, out var action)
            ? action
            : throw new NotSupportedException($"HTTP method '{method}' is not supported");
    }

    private static string GetResourceFromUrl(IContext context)
    {
        HttpContext httpContext = context.State.HttpContext;
        var route = httpContext.Request.Path.ToString().Split('/');
        string resource;
        if (route.Length > 1 &&
            (int.TryParse(route[^1], out _) ||
             Guid.TryParse(route[^1], out _)))
        {
            resource = route[^2] + "/" + route[^1];
        }
        else
        {
            resource = route[^1];
        }

        return resource;
    }

    private const string BearerPrefix = "Bearer ";

    private static string? GetUserIdFromToken(IContext context)
    {
        HttpContext httpContext = context.State.HttpContext;

        string? authorization = httpContext.Request.Headers.Authorization;
        if (string.IsNullOrEmpty(authorization))
        {
            throw new HttpRequestException("Authorization header is missing", null, HttpStatusCode.Unauthorized);
        }

        if (!authorization.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new HttpRequestException("Invalid authorization scheme", null, HttpStatusCode.Unauthorized);
        }

        var accessToken = authorization[BearerPrefix.Length..].Trim();
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new HttpRequestException("Access token is empty", null, HttpStatusCode.Unauthorized);
        }

        var jwtService = context.Services.GetRequiredService<IJwtService>();
        return jwtService.GetUserIdFromToken(accessToken);
    }
}