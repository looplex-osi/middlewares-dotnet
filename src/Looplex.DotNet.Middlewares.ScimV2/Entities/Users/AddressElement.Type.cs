using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

public partial class AddressElement
{
    /// <summary>
    ///     The country name component.  When specified, the value MUST be in ISO 3166-1 'alpha-2'
    ///     code format [ISO3166]; e.g., the United States and Sweden are 'US' and 'SE', respectively.
    /// </summary>
    [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
    public string Country { get; set; }

    /// <summary>
    ///     The full mailing address, formatted for display or use with a mailing label.  This
    ///     attribute MAY contain newlines.
    /// </summary>
    [JsonProperty("formatted", NullValueHandling = NullValueHandling.Ignore)]
    public string Formatted { get; set; }

    /// <summary>
    ///     The city or locality component.
    /// </summary>
    [JsonProperty("locality", NullValueHandling = NullValueHandling.Ignore)]
    public string Locality { get; set; }

    /// <summary>
    ///     The zip code or postal code component.
    /// </summary>
    [JsonProperty("postalCode", NullValueHandling = NullValueHandling.Ignore)]
    public string PostalCode { get; set; }

    /// <summary>
    ///     The state or region component.
    /// </summary>
    [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
    public string Region { get; set; }

    /// <summary>
    ///     The full street address component, which may include house number, street name, P.O. box,
    ///     and multi-line extended street address information.  This attribute MAY contain newlines.
    /// </summary>
    [JsonProperty("streetAddress", NullValueHandling = NullValueHandling.Ignore)]
    public string StreetAddress { get; set; }

    /// <summary>
    ///     A label indicating the attribute's function, e.g., 'work' or 'home'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public AddressType? Type { get; set; }
}