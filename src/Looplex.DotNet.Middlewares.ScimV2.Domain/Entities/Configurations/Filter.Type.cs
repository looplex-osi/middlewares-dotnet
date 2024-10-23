using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

/// <summary>
/// A complex type that specifies FILTER options.
/// </summary>
public partial class Filter
{
    /// <summary>
    /// An integer value specifying the maximum number of resources returned in a response.
    /// </summary>
    [JsonProperty("maxResults")]
    public double MaxResults { get; set; }

    /// <summary>
    /// A Boolean value specifying whether or not the operation is supported.
    /// </summary>
    [JsonProperty("supported")]
    public bool Supported { get; set; }
}