using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
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

        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Basic "))
        {
            accesToken = authorization["Basic ".Length..].Trim();
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