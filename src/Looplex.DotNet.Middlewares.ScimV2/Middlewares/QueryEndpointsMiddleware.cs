using System.Net;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.ScimV2.Middlewares;

public static partial class QueryEndpointsMiddleware
{
    public static readonly MiddlewareDelegate AttributesMiddleware = new(async (context, cancellationToken, next) =>
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var configuration = context.Services.GetService<IConfiguration>()!;
        var audience = configuration["Audience"]!;
        var issuer = configuration["Issuer"]!;

        string accesToken = string.Empty;

        HttpContext httpContext = context.State.HttpContext;

        string? authorization = httpContext.Request.Headers.Authorization;

        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        {
            accesToken = authorization["Bearer ".Length..].Trim();
        }

        var publicKey = StringUtils.Base64Decode(configuration["PublicKey"]!);
        
        var jwtService = context.Services.GetService<IJwtService>()!;
        bool authenticated = jwtService.ValidateToken(publicKey, issuer, audience, accesToken);

        if (!authenticated)
        {
            throw new HttpRequestException("AccessToken is invalid.", null, HttpStatusCode.Unauthorized);
        }

        await next();
    });
}