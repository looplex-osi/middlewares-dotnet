using Looplex.DotNet.Core.Domain.Entities;
using Looplex.DotNet.Core.Domain.Traits;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class EmailElement : IEntity, IHasChangedPropertyNotificationTrait
{
    /// <summary>
    ///     Sequencial id for an entity.
    /// </summary>
    [JsonIgnore]
    public virtual int? Id { get; set; }

    /// <summary>
    ///     A unique identifier for an entity.
    /// </summary>
    [JsonProperty("uuid")]
    public virtual Guid? UniqueId { get; set; }
    
    [JsonIgnore]
    public virtual int? UserId { get; set; }

    /// <summary>
    ///     A Boolean value indicating the 'primary' or preferred attribute value for this attribute,
    ///     e.g., the preferred mailing email or primary email. The primary attribute
    ///     value 'true' MUST appear no more than once.
    /// </summary>
    [JsonProperty("primary", NullValueHandling = NullValueHandling.Ignore)]
    public virtual bool? Primary { get; set; }

    /// <summary>
    ///     A label indicating the attribute's function, e.g., 'work' or 'home'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public virtual EmailType? Type { get; set; }

    /// <summary>
    ///     Email addresses for the user. The value SHOULD be canonicalized by the service provider,
    ///     e.g., 'bjensen@example.com' instead of 'bjensen@EXAMPLE.COM'. Canonical type values of
    ///     'work', 'home', and 'other'.
    /// </summary>
    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? Value { get; set; }
}