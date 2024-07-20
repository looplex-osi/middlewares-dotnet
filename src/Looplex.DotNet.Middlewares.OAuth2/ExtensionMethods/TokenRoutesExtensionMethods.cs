using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;

public static class TokenRoutesExtensionMethods
{
    private const string Resource = "/token";
    private const string Tag = "Authentication";

    private static readonly MiddlewareDelegate TokenMiddleware = new(async (context, cancellationToken, _) =>
    {
        IAuthorizationService service = context.Services.GetRequiredService<IAuthorizationService>();

        HttpContext httpContext = context.State.HttpContext;
        context.State.Authorization = httpContext.Request.Headers.Authorization.ToString();
            
        using StreamReader reader = new(httpContext.Request.Body);
        context.State.Resource = await reader.ReadToEndAsync(cancellationToken);
            
        await service.CreateAccessToken(context, cancellationToken);

        await httpContext.Response.WriteAsJsonAsync(context.Result, cancellationToken);
    });

    public static void UseTokenRoute(this IEndpointRouteBuilder app, string[] services)
    {
        app.MapPost(
                Resource,
                new RouteBuilderOptions
                {
                    Services = services,
                    Middlewares =
                    [
                        CoreMiddlewares.ExceptionMiddleware,
                        TokenMiddleware
                    ]
                })
            .WithTags(Tag);
    }
}