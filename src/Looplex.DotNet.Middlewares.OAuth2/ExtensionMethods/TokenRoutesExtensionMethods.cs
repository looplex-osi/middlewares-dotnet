using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Factories;
using Looplex.DotNet.Middlewares.OAuth2.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Looplex.DotNet.Middlewares.OAuth2.Domain.ExtensionMethods;

namespace Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;

public static class TokenRoutesExtensionMethods
{
    private const string Resource = "/token";

    internal static readonly MiddlewareDelegate TokenMiddleware = async (context, _) =>
    {
        IAuthorizationServiceFactory factory = context.Services.GetRequiredService<IAuthorizationServiceFactory>();

        var cancellationToken = context.GetRequiredValue<CancellationToken>("CancellationToken");
        var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
        context.State.Authorization = httpContext.Request.Headers.Authorization.ToString();
            
        var form = await httpContext.Request.ReadFormAsync();
        var formDict = form.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

        context.State.Resource = JsonConvert.SerializeObject(formDict);

        var grantType = form[Constants.GrantType].ToString().ToGrantType();
        var service = factory.GetService(grantType);
        
        await service.CreateAccessToken(context, cancellationToken);

        await httpContext.Response.WriteAsJsonAsync(context.Result, cancellationToken);
    };

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
            });
    }
}