using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.OAuth2.Middlewares;

public static partial class OAuth2Middlewares
{
    public static readonly MiddlewareDelegate AuthenticationMiddleware = async (context, next) =>
    {
        var cancellationToken = context.GetRequiredValue<CancellationToken>("CancellationToken");
        cancellationToken.ThrowIfCancellationRequested();

        var configuration = context.Services.GetRequiredService<IConfiguration>();
        var audience = configuration["Audience"] ??
                       throw new InvalidOperationException("Audience configuration is missing");
        var issuer = configuration["Issuer"] ??
                     throw new InvalidOperationException("Issuer configuration is missing");

        string accessToken = string.Empty;

        var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");

        string? authorization = httpContext.Request.Headers.Authorization;

        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer ", StringComparison.Ordinal))
        {
            accessToken = authorization["Bearer ".Length..].Trim();
        }

        var publicKey = StringUtils.Base64Decode(configuration["PublicKey"] ??
                                                 throw new InvalidOperationException(
                                                     "PublicKey configuration is missing"));
        var jwtService = context.Services.GetRequiredService<IJwtService>();
        bool authenticated = jwtService.ValidateToken(publicKey, issuer, audience, accessToken);

        if (!authenticated)
        {
            throw new HttpRequestException("AccessToken is invalid.", null, HttpStatusCode.Unauthorized);
        }
        
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);
        
        var claims = token.Claims.ToList();

        if (claims.Any(c => c.Type is "name" or "email"))
        {
            var name = claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;

            context.State.User = new ExpandoObject();
#pragma warning disable CS8601 // Possible null reference assignment.
            context.State.User.Name = name;
            context.State.User.Email = email;
#pragma warning restore CS8601 // Possible null reference assignment.
        }
        
        await next();
    };
}