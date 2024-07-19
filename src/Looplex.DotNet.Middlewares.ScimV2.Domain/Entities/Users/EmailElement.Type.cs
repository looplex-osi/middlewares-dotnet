using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class EmailElement
{
    /// <summary>
    ///     A Boolean value indicating the 'primary' or preferred attribute value for this attribute,
    ///     e.g., the preferred mailing email or primary email email.  The primary attribute
    ///     value 'true' MUST appear no more than once.
    /// </summary>
    [JsonProperty("primary", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Primary { get; set; }

    /// <summary>
    ///     A label indicating the attribute's function, e.g., 'work' or 'home'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public EmailType? Type { get; set; }

    /// <summary>
    ///     Email emailes for the user.  The value SHOULD be canonicalized by the service provider,
    ///     e.g., 'bjensen@example.com' instead of 'bjensen@EXAMPLE.COM'. Canonical type values of
    ///     'work', 'home', and 'other'.
    /// </summary>
    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public string? Value { get; set; }
}