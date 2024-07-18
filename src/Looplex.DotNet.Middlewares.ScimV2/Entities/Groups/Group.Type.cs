using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Groups;

public partial class Group : Resource
{
    [JsonProperty("displayName")] public string DisplayName { get; set; }

    /// <summary>
    ///     A String that is an identifier for the resource as defined by the provisioning client.
    /// </summary>
    [JsonProperty("externalId", NullValueHandling = NullValueHandling.Ignore)]
    public string ExternalId { get; set; }

    /// <summary>
    ///     A unique identifier for a SCIM resource as defined by the service provider.
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("members")] public List<MemberElement> Members { get; set; }

    [JsonProperty("meta", NullValueHandling = NullValueHandling.Ignore)]
    public Meta Meta { get; set; }
}