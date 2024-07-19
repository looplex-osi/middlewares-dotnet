using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.OAuth2.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.OAuth2;

public static partial class AuthenticationMiddlewares
{
    public static readonly MiddlewareDelegate AuthenticateMiddleware = new(async (context, cancellationToken, next) =>
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

        using var jwtService = new JwtService(
            StringUtils.Base64Decode(configuration["PrivateKey"]!),
            StringUtils.Base64Decode(configuration["PublicKey"]!));
        bool authenticated = jwtService.ValidateToken(issuer, audience, accesToken);

        if (!authenticated)
        {
            throw new HttpRequestException("AccessToken is invalid.", null, HttpStatusCode.Unauthorized);
        }

        await next();
    });
}