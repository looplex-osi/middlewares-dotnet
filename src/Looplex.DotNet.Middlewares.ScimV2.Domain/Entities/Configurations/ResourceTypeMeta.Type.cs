using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

public partial class ResourceTypeMeta
{
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
}