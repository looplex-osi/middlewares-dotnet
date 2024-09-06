using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Abstractions;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class PhotoElement : ObservableType, IEntity
{
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
    ///     A Boolean value indicating the 'primary' or preferred attribute value for this attribute,
    ///     e.g., the preferred photo or thumbnail.  The primary attribute value 'true' MUST appear
    ///     no more than once.
    /// </summary>
    [JsonProperty("primary", NullValueHandling = NullValueHandling.Ignore)]
    public virtual bool? Primary { get; set; }

    /// <summary>
    ///     A label indicating the attribute's function, i.e., 'photo' or 'thumbnail'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public virtual PhotoType? Type { get; set; }

    /// <summary>
    ///     URL of a photo of the User.
    /// </summary>
    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public virtual Uri? Value { get; set; }
}