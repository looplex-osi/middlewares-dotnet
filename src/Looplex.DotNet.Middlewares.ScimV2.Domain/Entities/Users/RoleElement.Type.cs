using Looplex.DotNet.Core.Domain.Entities;
using Looplex.DotNet.Core.Domain.Traits;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class RoleElement : IEntity, IHasChangedPropertyNotificationTrait
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
    ///     A Boolean value indicating the 'primary' or preferred attribute value for this
    ///     attribute.  The primary attribute value 'true' MUST appear no more than once.
    /// </summary>
    [JsonProperty("primary", NullValueHandling = NullValueHandling.Ignore)]
    public virtual bool? Primary { get; set; }

    /// <summary>
    ///     A label indicating the attribute's function.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? Type { get; set; }

    /// <summary>
    ///     The value of a role.
    /// </summary>
    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? Value { get; set; }
}