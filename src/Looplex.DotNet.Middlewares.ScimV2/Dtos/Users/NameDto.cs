using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.Dtos.Users
{
    public class NameDto
    {
        [JsonPropertyName("familyName")]
        public required string FamilyName { get; set; }

        [JsonPropertyName("givenName")]
        public required string GivenName { get; set; }

        [JsonPropertyName("middleName")]
        public string? MiddleName { get; set; }

        [JsonPropertyName("honorificPrefix")]
        public string? HonorificPrefix { get; set; }

        [JsonPropertyName("honorificSuffix")]
        public string? HonorificSuffix { get; set; }
    }
}
