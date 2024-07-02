using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.DTOs.Users
{
    public class UserGroupDTO
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("$ref")]
        public string Ref { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
