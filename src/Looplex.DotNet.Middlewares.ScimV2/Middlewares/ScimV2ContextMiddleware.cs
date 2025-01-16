using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;
using Microsoft.AspNetCore.Http;

namespace Looplex.DotNet.Middlewares.ScimV2.Middlewares;

public static partial class ScimV2Middlewares
{
    const string LooplexTenantKeyHeader = "X-looplex-tenant";
    
    public static readonly MiddlewareDelegate ScimV2ContextMiddleware = async (context, next) =>
    {
        var httpContext = context.GetRequiredValue<HttpContext>("HttpContext");
        
        context.AsScimV2Context().RouteValues = httpContext.Request.RouteValues
            .Select(rv => new KeyValuePair<string, object?>(rv.Key, rv.Value))
            .ToDictionary();
        context.AsScimV2Context().Query = httpContext.Request.Query
            .Select(q => new KeyValuePair<string, string>(q.Key, q.Value.ToString())).ToDictionary();
        context.AsScimV2Context().Headers = httpContext.Request.Headers
            .Select(q => new KeyValuePair<string, string>(q.Key, q.Value.ToString())).ToDictionary();

        if (context.AsScimV2Context().Headers.TryGetValue(LooplexTenantKeyHeader, out var tenant))
        {
            context.State.Tenant = tenant;
        }
        
        await next();
    };
}