using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

public partial class PhotoElement
{
    /// <summary>
    ///     A Boolean value indicating the 'primary' or preferred attribute value for this attribute,
    ///     e.g., the preferred photo or thumbnail.  The primary attribute value 'true' MUST appear
    ///     no more than once.
    /// </summary>
    [JsonProperty("primary", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Primary { get; set; }

    /// <summary>
    ///     A label indicating the attribute's function, i.e., 'photo' or 'thumbnail'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public PhotoType? Type { get; set; }

    /// <summary>
    ///     URL of a photo of the User.
    /// </summary>
    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public Uri? Value { get; set; }
}