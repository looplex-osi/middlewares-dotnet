using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

/// <summary>
/// A complex type that specifies ETag configuration options.
/// </summary>
public partial class Etag
{
    /// <summary>
    /// A Boolean value specifying whether or not the operation is supported.
    /// </summary>
    [JsonProperty("supported")]
    public bool Supported { get; set; }
}