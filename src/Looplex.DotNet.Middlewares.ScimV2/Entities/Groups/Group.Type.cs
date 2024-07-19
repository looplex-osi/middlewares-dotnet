using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Groups;

public partial class Group : Resource
{
    [JsonProperty("displayName")] public string? DisplayName { get; set; }

    [JsonProperty("members")] public List<MemberElement> Members { get; set; } = [];
}