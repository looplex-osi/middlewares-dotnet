using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class EntitlementElement
{
    [JsonIgnore]
    public required string Id { get; set; }
    
    [JsonIgnore]
    public required string UserId { get; set; }
    
    /// <summary>
    ///     A Boolean value indicating the 'primary' or preferred attribute value for this
    ///     attribute.  The primary attribute value 'true' MUST appear no more than once.
    /// </summary>
    [JsonProperty("primary", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Primary { get; set; }

    /// <summary>
    ///     A label indicating the attribute's function.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type { get; set; }

    /// <summary>
    ///     The value of an entitlement.
    /// </summary>
    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public string? Value { get; set; }
}