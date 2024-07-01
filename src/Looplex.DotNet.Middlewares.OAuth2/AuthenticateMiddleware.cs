using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.OAuth2.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Looplex.DotNet.Middlewares.OAuth2
{
    public static partial class AuthenticationMiddlewares
    {
        public readonly static MiddlewareDelegate AuthenticateMiddleware = new(async (context, next) =>
        {
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
}
