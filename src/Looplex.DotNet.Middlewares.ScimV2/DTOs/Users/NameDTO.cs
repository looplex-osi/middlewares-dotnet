using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.DTOs.Users
{
    public class NameDTO
    {
        [JsonPropertyName("formatted")]
        public string Formatted { get; set; }

        [JsonPropertyName("familyName")]
        public string FamilyName { get; set; }

        [JsonPropertyName("givenName")]
        public string GivenName { get; set; }

        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; }

        [JsonPropertyName("honorificPrefix")]
        public string HonorificPrefix { get; set; }

        [JsonPropertyName("honorificSuffix")]
        public string HonorificSuffix { get; set; }
    }
}
