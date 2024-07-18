using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Looplex.DotNet.Middlewares.OAuth2.DTOs;
using Looplex.DotNet.Middlewares.OAuth2.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Looplex.DotNet.Core.WebAPI.Middlewares;

namespace Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods
{
    public static class TokenRoutesExtensionMethods
    {
        private const string RESOURCE = "/token";
        private const string TAG = "Authentication";

        internal readonly static MiddlewareDelegate TokenMiddleware = new(async (context, next) =>
        {
            IConfiguration configuration = context.Services.GetRequiredService<IConfiguration>();
            IAuthorizationService service = context.Services.GetRequiredService<IAuthorizationService>();

            HttpContext httpContext = context.State.HttpContext;
            
            string? authorization = httpContext.Request.Headers.Authorization;

            using StreamReader reader = new(httpContext.Request.Body);
            var clientCredentialsDTO = JsonSerializer.Deserialize<ClientCredentialsDTO>(await reader.ReadToEndAsync(), JsonUtils.HttpBodyConverter())!;

            context.State.Authorization = authorization;
            context.State.ClientCredentialsDTO = clientCredentialsDTO;
            await service.CreateAccessToken(context);

            await httpContext.Response.WriteAsJsonAsync(new AccessTokenDTO
            {
                AccessToken = (string)context.Result,
            });
        });

        public static void UseTokenRoute(this IEndpointRouteBuilder app, string[] services)
        {
            app.MapPost(
                RESOURCE,
                new RouteBuilderOptions
                {
                    Services = services,
                    Middlewares = [
                        CoreMiddlewares.ExceptionMiddleware,
                        TokenMiddleware
                    ]
                })
            .WithTags(TAG)
            .Accepts<ClientCredentialsDTO>(JsonUtils.JsonContentTypeWithCharset)
            .Produces<AccessTokenDTO>(StatusCodes.Status200OK);
        }
    }
}
