using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

public partial class GroupElement
{
    /// <summary>
    ///     The URI of the corresponding 'Group' resource to which the user belongs.
    /// </summary>
    [JsonProperty("$ref", NullValueHandling = NullValueHandling.Ignore)]
    public Uri Ref { get; set; }

    /// <summary>
    ///     A label indicating the attribute's function, e.g., 'direct' or 'indirect'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public GroupType? Type { get; set; }

    /// <summary>
    ///     The identifier of the User's group.
    /// </summary>
    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public string Value { get; set; }
}