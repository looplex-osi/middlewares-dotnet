using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;

public partial class MemberElement
{
    [JsonIgnore]
    public required string Id { get; set; }
    
    [JsonIgnore]
    public required string GroupId { get; set; }
    
    /// <summary>
    ///     The URI corresponding to a SCIM resource that is a member of this Group.
    /// </summary>
    [JsonProperty("$ref", NullValueHandling = NullValueHandling.Ignore)]
    public Uri? Ref { get; set; }

    /// <summary>
    ///     A label indicating the type of resource, e.g., 'User' or 'Group'.
    /// </summary>
    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public GroupType? Type { get; set; }

    /// <summary>
    ///     Identifier of the member of this Group.
    /// </summary>
    [JsonProperty("value")]
    public string? Value { get; set; }
}