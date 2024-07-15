using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.Dtos.Users
{
    public class UserGroupDto
    {
        [JsonPropertyName("value")]
        public required string Value { get; set; }

        [JsonPropertyName("$ref")]
        public string? Ref { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
