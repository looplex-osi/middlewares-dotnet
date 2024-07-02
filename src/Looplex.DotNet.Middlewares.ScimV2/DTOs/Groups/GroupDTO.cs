using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.DTOs.Groups
{
    public class GroupDTO
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
    }
}
