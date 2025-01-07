using System.Collections.Specialized;
using System.Web;

namespace Looplex.DotNet.Middlewares.ScimV2.Utils;

public static class PaginationUtils
{
    public static string CreateLinkHeader(Uri uri, int startIndex, int count, int totalResults)
    {
        var links = new List<string>();

        NameValueCollection queryParams = HttpUtility.ParseQueryString(uri.Query);

        // Create link for self page
        queryParams["startIndex"] = $"{startIndex}";
        var selfUri = new UriBuilder(uri)
        {
            Query = queryParams.ToString(),
        };
        links.Add($"<{selfUri}>; rel=\"self\"");

        // Create link for first page
        var firstUri = new UriBuilder(uri)
        {
            Query = $"?count={count}&startIndex=1"
        };
        links.Add($"<{firstUri}>; rel=\"first\"");

        // Create link for previous page
        var prevPageStartIndex = startIndex - count;
        if (prevPageStartIndex > 0)
        {
            queryParams["startIndex"] = $"{prevPageStartIndex}";
            var prevUri = new UriBuilder(uri)
            {
                Query = queryParams.ToString(),
            };
            links.Add($"<{prevUri}>; rel=\"prev\"");
        }

        // Create link for next page
        var nextPageStartIndex = startIndex + count;
        if (nextPageStartIndex < totalResults)
        {
            queryParams["startIndex"] = $"{nextPageStartIndex}";
            var nextUri = new UriBuilder(uri)
            {
                Query = queryParams.ToString(),
            };
            links.Add($"<{nextUri}>; rel=\"next\"");
        }

        // Create link for last page
        var lastPageStartIndex = totalResults - count + 1;
        if (lastPageStartIndex > 0)
        {
            queryParams["startIndex"] = $"{lastPageStartIndex}";
            var lastUri = new UriBuilder(uri)
            {
                Query = queryParams.ToString(),
            };
            links.Add($"<{lastUri}>; rel=\"last\"");
        }

        return string.Join(", ", links);
    }
}