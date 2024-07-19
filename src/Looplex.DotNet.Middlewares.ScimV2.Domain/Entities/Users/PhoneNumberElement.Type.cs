using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class PhoneNumberElement
{
    /// <summary>
    ///     A Boolean value indicating the 'primary' or preferred attribute value for this attribute,
    ///     e.g., the preferred phone number or primary phone number.  The primary attribute value
    ///     'true' MUST appear no more than once.
    /// </summary>
    [JsonProperty("primary", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Primary { get; set; }

    /// <summary>
    ///     A label indicating the attribute's function, e.g., 'work', 'home', 'mobile'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public PhoneNumberType? Type { get; set; }

    /// <summary>
    ///     Phone number of the User.
    /// </summary>
    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public string? Value { get; set; }
}