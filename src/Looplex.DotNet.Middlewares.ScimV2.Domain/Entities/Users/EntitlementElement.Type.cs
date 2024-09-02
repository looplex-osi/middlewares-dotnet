using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class EntitlementElement
{private bool? _primary;
    private string? _type;
    private string? _value;

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
    ///     A Boolean value indicating the 'primary' or preferred attribute value for this
    ///     attribute. The primary attribute value 'true' MUST appear no more than once.
    /// </summary>
    [JsonProperty("primary", NullValueHandling = NullValueHandling.Ignore)]
    public bool? Primary
    {
        get => _primary;
        set
        {
            if (value != _primary)
            {
                _primary = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     A label indicating the attribute's function.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type
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

    /// <summary>
    ///     The value of an entitlement.
    /// </summary>
    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public string? Value
    {
        get => _value;
        set
        {
            if (value != _value)
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }
}