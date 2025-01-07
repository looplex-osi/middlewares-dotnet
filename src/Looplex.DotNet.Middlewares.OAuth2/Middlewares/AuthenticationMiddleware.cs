using System.Net;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.OAuth2.Middlewares;

public static partial class AuthenticationMiddleware
{
    public static readonly MiddlewareDelegate AuthenticateMiddleware = new(async (context, cancellationToken, next) =>
    {
        cancellationToken.ThrowIfCancellationRequested();

        var configuration = context.Services.GetRequiredService<IConfiguration>();
        var audience = configuration["Audience"] ??
            throw new InvalidOperationException("Audience configuration is missing");
        var issuer = configuration["Issuer"] ??
            throw new InvalidOperationException("Issuer configuration is missing");

        string accesToken = string.Empty;

        HttpContext httpContext = context.State.HttpContext;

        string? authorization = httpContext.Request.Headers.Authorization;

        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer ", StringComparison.Ordinal))
        {
            accesToken = authorization["Bearer ".Length..].Trim();
        }

        var publicKey = StringUtils.Base64Decode(configuration["PublicKey"] ??
            throw new InvalidOperationException("PublicKey configuration is missing"));
        var jwtService = context.Services.GetRequiredService<IJwtService>();
        bool authenticated = jwtService.ValidateToken(publicKey, issuer, audience, accesToken);

        if (!authenticated)
        {
            throw new HttpRequestException("AccessToken is invalid.", null, HttpStatusCode.Unauthorized);
        }

        await next();
    });
}