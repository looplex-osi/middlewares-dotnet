using Looplex.DotNet.Core.Domain.Entities;
using Looplex.DotNet.Core.Domain.Traits;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;

public partial class MemberElement : IEntity, IHasChangedPropertyNotificationTrait
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
    public virtual int? GroupId { get; set; }
    
    /// <summary>
    ///     The URI corresponding to a SCIM resource that is a member of this Group.
    /// </summary>
    [JsonProperty("$ref", NullValueHandling = NullValueHandling.Ignore)]
    public virtual Uri? Ref { get; set; }

    /// <summary>
    ///     A label indicating the type of resource, e.g., 'User' or 'Group'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public virtual GroupType? Type { get; set; }

    /// <summary>
    ///     Identifier of the member of this Group.
    /// </summary>
    [JsonProperty("value")]
    public virtual string? Value { get; set; }
}