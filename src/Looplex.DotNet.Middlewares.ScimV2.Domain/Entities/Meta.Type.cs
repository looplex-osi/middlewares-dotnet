using Looplex.DotNet.Core.Domain.Traits;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;

public partial class Meta : IHasChangedPropertyNotificationTrait
{
    /// <summary>
    ///     The `DateTimeOffset` that the resource was added to the service provider. This attribute MUST
    ///     be a DateTimeOffset ISO8601Z.
    /// </summary>
    [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
    public virtual DateTimeOffset? Created { get; set; }

    /// <summary>
    ///     The most recent DateTimeOffset that the details of this resource were updated at the service
    ///     provider. If this resource has never been modified since its initial creation, the value
    ///     MUST be the same as the value of `created`.
    /// </summary>
    [JsonProperty("lastModified", NullValueHandling = NullValueHandling.Ignore)]
    public virtual DateTimeOffset? LastModified { get; set; }

    /// <summary>
    ///     The URI of the resource being returned. This value MUST be the same as the
    ///     `Content-Location` HTTP response header (see Section 3.1.4.2 of [RFC7231]).
    /// </summary>
    [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
    public virtual Uri? Location { get; set; }

    /// <summary>
    ///     The name of the resource type of the resource. This attribute has a mutability of
    ///     `readOnly` and `caseExact` as `true`.
    /// </summary>
    [JsonProperty("resourceType", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? ResourceType { get; set; }

    /// <summary>
    ///     The version of the resource being returned. This value must be the same as the
    ///     entity-tag (ETag) HTTP response header (see Sections 2.1 and 2.3 of [RFC7232]).
    /// </summary>
    [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string? Version { get; set; }
}