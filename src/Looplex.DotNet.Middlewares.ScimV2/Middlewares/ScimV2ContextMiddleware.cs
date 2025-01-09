using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;
using Microsoft.AspNetCore.Http;

namespace Looplex.DotNet.Middlewares.ScimV2.Middlewares;

public static partial class ScimV2Middlewares
{
    public static readonly MiddlewareDelegate ScimV2ContextMiddleware = async (context, cancellationToken, next) =>
    {
        cancellationToken.ThrowIfCancellationRequested();
        HttpContext httpContext = context.State.HttpContext;
        
        context.AsScimV2Context().RouteValues = httpContext.Request.RouteValues
            .Select(rv => new KeyValuePair<string, object?>(rv.Key, rv.Value))
            .ToDictionary();
        context.AsScimV2Context().Query = httpContext.Request.Query
            .Select(q => new KeyValuePair<string, string>(q.Key, q.Value.ToString())).ToDictionary();
        context.AsScimV2Context().Headers = httpContext.Request.Headers
            .Select(q => new KeyValuePair<string, string>(q.Key, q.Value.ToString())).ToDictionary();
        
        await next();
    };
}