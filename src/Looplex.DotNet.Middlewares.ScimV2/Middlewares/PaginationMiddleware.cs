using System.Dynamic;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.ScimV2.Utils;
using Microsoft.AspNetCore.Http;

namespace Looplex.DotNet.Middlewares.ScimV2.Middlewares;

public static partial class ScimV2Middlewares
{
    private const string ErrTemplate = "Param {0} must be specified for paginated resources";
    private const string ErrTypeTemplate = "Param {0} is not valid";
    private const int MinValue = 1;

    /// <summary>
    /// Pagination parameters can be used together to "page through" large
    /// numbers of resources so as not to overwhelm the client or service
    /// provider.  Because pagination is not stateful, clients MUST be
    /// prepared to handle inconsistent results.  For example, a request for
    /// a list of 10 resources beginning with a startIndex of 1 MAY return
    /// different results when repeated, since resources on the service
    /// provider may have changed between requests.  Pagination parameters
    /// and general behavior are derived from the OpenSearch Protocol
    /// [OpenSearch].
    /// <seealso cref="https://datatracker.ietf.org/doc/html/rfc7644#ref-OpenSearch"/>
    /// </summary>
    /// <see cref="https://datatracker.ietf.org/doc/html/rfc7644#section-3.4.2.4"/>
    public static readonly MiddlewareDelegate PaginationMiddleware = async (context, cancellationToken, next) =>
    {
        cancellationToken.ThrowIfCancellationRequested();
        HttpContext httpContext = context.State.HttpContext;

        int startIndex = GetQueryParam(httpContext, "startIndex");
        int count = GetQueryParam(httpContext, "count");

        context.State.Pagination = new ExpandoObject();
        context.State.Pagination.StartIndex = startIndex;
        context.State.Pagination.ItemsPerPage = count;

        await next();

        var uri = GetUri(httpContext.Request);

        var totalCount = (int)context.State.Pagination.TotalCount;
        var linkHeader = PaginationUtils.CreateLinkHeader(uri, startIndex, count, totalCount);

        httpContext.Response.Headers.Append("Link", linkHeader);
    };

    private static int GetQueryParam(HttpContext httpContext, string param)
    {
        string value;
        if (httpContext.Request.Query.TryGetValue(param, out var stringValue ))
        {
            value = stringValue.ToString();
        }
        else
        {
            throw new ArgumentNullException(string.Format(ErrTemplate, param));
        }

        if (!int.TryParse(value, out var intValue) || intValue < MinValue)
        {
            throw new ArgumentException(string.Format(ErrTypeTemplate, param));
        }

        return intValue;
    }
        
    private static Uri GetUri(this HttpRequest request)
    {
        var uriBuilder = new UriBuilder
        {
            Scheme = request.Scheme,
            Host = request.Host.Host,
            Port = request.Host.Port.GetValueOrDefault(80),
            Path = request.Path.ToString(),
            Query = request.QueryString.ToString()
        };
        return uriBuilder.Uri;
    }
}