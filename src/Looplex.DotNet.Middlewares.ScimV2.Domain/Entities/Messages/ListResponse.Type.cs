using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;

public partial class ListResponse
{
    /// <summary>
    /// The total number of results returned by the list or query operation. This value may be larger than the number of resources returned if pagination is used. REQUIRED.
    /// </summary>
    [JsonProperty("totalResults")]
    public long TotalResults { get; set; }
        
    /// <summary>
    /// A list of complex objects containing the requested resources. REQUIRED if 'totalResults' is non-zero.
    /// </summary>
    [JsonProperty("Resources")]
    public List<object> Resources { get; set; } = [];
        
    /// <summary>
    /// The 1-based index of the first result in the current set of list results. REQUIRED when partial results are returned due to pagination.
    /// </summary>
    [JsonProperty("startIndex")]
    public long StartIndex { get; set; }

    /// <summary>
    /// The number of resources returned in a list response page. REQUIRED when partial results are returned due to pagination.
    /// </summary>
    [JsonProperty("itemsPerPage")]
    public long ItemsPerPage { get; set; }
}