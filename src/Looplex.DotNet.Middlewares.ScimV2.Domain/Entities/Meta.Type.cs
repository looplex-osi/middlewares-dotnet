using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;

public partial class Meta
{
    private DateTimeOffset? _created;
    private DateTimeOffset? _lastModified;
    private Uri? _location;
    private string? _resourceType;
    private string? _version;

    /// <summary>
    ///     The `DateTimeOffset` that the resource was added to the service provider. This attribute MUST
    ///     be a DateTimeOffset ISO8601Z.
    /// </summary>
    [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? Created
    {
        get => _created;
        set
        {
            if (value != _created)
            {
                _created = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The most recent DateTimeOffset that the details of this resource were updated at the service
    ///     provider. If this resource has never been modified since its initial creation, the value
    ///     MUST be the same as the value of `created`.
    /// </summary>
    [JsonProperty("lastModified", NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? LastModified
    {
        get => _lastModified;
        set
        {
            if (value != _lastModified)
            {
                _lastModified = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The URI of the resource being returned. This value MUST be the same as the
    ///     `Content-Location` HTTP response header (see Section 3.1.4.2 of [RFC7231]).
    /// </summary>
    [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
    public Uri? Location
    {
        get => _location;
        set
        {
            if (value != _location)
            {
                _location = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The name of the resource type of the resource. This attribute has a mutability of
    ///     `readOnly` and `caseExact` as `true`.
    /// </summary>
    [JsonProperty("resourceType", NullValueHandling = NullValueHandling.Ignore)]
    public string? ResourceType
    {
        get => _resourceType;
        set
        {
            if (value != _resourceType)
            {
                _resourceType = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    ///     The version of the resource being returned. This value must be the same as the
    ///     entity-tag (ETag) HTTP response header (see Sections 2.1 and 2.3 of [RFC7232]).
    /// </summary>
    [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
    public string? Version
    {
        get => _version;
        set
        {
            if (value != _version)
            {
                _version = value;
                OnPropertyChanged();
            }
        }
    }
}