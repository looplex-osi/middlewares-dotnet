using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

public partial class ImElement
{
    /// <summary>
    ///     A Boolean value indicating the 'primary' or preferred attribute value for this attribute,
    ///     e.g., the preferred messenger or primary messenger.  The primary attribute value 'true'
    ///     MUST appear no more than once.
    /// </summary>
    [JsonProperty("primary", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Primary { get; set; }

    /// <summary>
    ///     A label indicating the attribute's function, e.g., 'aim', 'gtalk', 'xmpp'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type { get; set; }

    /// <summary>
    ///     Instant messaging address for the User.
    /// </summary>
    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public string? Value { get; set; }
}