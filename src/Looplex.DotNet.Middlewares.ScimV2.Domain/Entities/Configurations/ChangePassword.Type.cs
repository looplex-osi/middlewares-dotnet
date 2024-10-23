using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

/// <summary>
/// A complex type that specifies configuration options related to changing a password.
/// </summary>
public partial class ChangePassword
{
    /// <summary>
    /// A Boolean value specifying whether or not the operation is supported.
    /// </summary>
    [JsonProperty("supported")]
    public bool Supported { get; set; }
}