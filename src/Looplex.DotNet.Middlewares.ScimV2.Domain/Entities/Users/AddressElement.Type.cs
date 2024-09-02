using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class AddressElement
{
    private string? _country;
    private string? _formatted;
    private string? _locality;
    private string? _postalCode;
    private string? _region;
    private string? _streetAddress;
    private AddressType? _type;

    /// <summary>
    ///     Sequencial id for an entity.
    /// </summary>
    [JsonIgnore]
    public int? Id { get; set; }

    /// <summary>
    ///     A unique identifier for an entity.
    /// </summary>
    [JsonProperty("uuid")]
    public Guid? UniqueId { get; set; }
    
    [JsonIgnore]
    public int? UserId { get; set; }

    /// <summary>
    ///     The country name component.  When specified, the value MUST be in ISO 3166-1 'alpha-2'
    ///     code format [ISO3166]; e.g., the United States and Sweden are 'US' and 'SE', respectively.
    /// </summary>
    [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
    public string? Country
    {
        get => _country;
        set
        {
            if (value != _country)
            {
                _country = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The full mailing address, formatted for display or use with a mailing label.  This
    ///     attribute MAY contain newlines.
    /// </summary>
    [JsonProperty("formatted", NullValueHandling = NullValueHandling.Ignore)]
    public string? Formatted
    {
        get => _formatted;
        set
        {
            if (value != _formatted)
            {
                _formatted = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The city or locality component.
    /// </summary>
    [JsonProperty("locality", NullValueHandling = NullValueHandling.Ignore)]
    public string? Locality
    {
        get => _locality;
        set
        {
            if (value != _locality)
            {
                _locality = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The zip code or postal code component.
    /// </summary>
    [JsonProperty("postalCode", NullValueHandling = NullValueHandling.Ignore)]
    public string? PostalCode
    {
        get => _postalCode;
        set
        {
            if (value != _postalCode)
            {
                _postalCode = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The state or region component.
    /// </summary>
    [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
    public string? Region
    {
        get => _region;
        set
        {
            if (value != _region)
            {
                _region = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The full street address component, which may include house number, street name, P.O. box,
    ///     and multi-line extended street address information.  This attribute MAY contain newlines.
    /// </summary>
    [JsonProperty("streetAddress", NullValueHandling = NullValueHandling.Ignore)]
    public string? StreetAddress
    {
        get => _streetAddress;
        set
        {
            if (value != _streetAddress)
            {
                _streetAddress = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     A label indicating the attribute's function, e.g., 'work' or 'home'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public AddressType? Type
    {
        get => _type;
        set
        {
            if (value != _type)
            {
                _type = value;
                OnPropertyChanged();
            }
        }
    }
}