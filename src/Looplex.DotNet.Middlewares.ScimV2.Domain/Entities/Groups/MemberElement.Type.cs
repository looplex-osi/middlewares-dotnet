using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;

public partial class MemberElement
{
    private Uri? _ref;
    private GroupType? _type;
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
    public int? GroupId { get; set; }
    
    /// <summary>
    ///     The URI corresponding to a SCIM resource that is a member of this Group.
    /// </summary>
    [JsonProperty("$ref", NullValueHandling = NullValueHandling.Ignore)]
    public Uri? Ref
    {
        get => _ref;
        set
        {
            if (value != _ref)
            {
                _ref = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     A label indicating the type of resource, e.g., 'User' or 'Group'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public GroupType? Type
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
    ///     Identifier of the member of this Group.
    /// </summary>
    [JsonProperty("value")]
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