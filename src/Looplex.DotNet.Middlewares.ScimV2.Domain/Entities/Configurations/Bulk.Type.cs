using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

/// <summary>
/// A complex type that specifies bulk configuration options.  See Section 3.7 of [RFC7644].
/// </summary>
public partial class Bulk
{
    /// <summary>
    /// An integer value specifying the maximum number of operations.
    /// </summary>
    [JsonProperty("maxOperations")]
    public double MaxOperations { get; set; }

    /// <summary>
    /// An integer value specifying the maximum payload size in bytes.
    /// </summary>
    [JsonProperty("maxPayloadSize")]
    public double MaxPayloadSize { get; set; }

    /// <summary>
    /// A Boolean value specifying whether or not the operation is supported.
    /// </summary>
    [JsonProperty("supported")]
    public bool Supported { get; set; }
}