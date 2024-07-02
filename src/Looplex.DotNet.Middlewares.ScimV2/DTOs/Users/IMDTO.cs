using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.DTOs.Users
{
    public class IMDTO
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("primary")]
        public bool Primary { get; set; } = false;
    }
}
