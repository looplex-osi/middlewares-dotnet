using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.Dtos
{
    public class MetaDto
    {
        [JsonPropertyName("resourceType")]
        public required string ResourceType { get; set; }

        [JsonPropertyName("created")]
        public DateTimeOffset Created { get; set; }

        [JsonPropertyName("lastModified")]
        public DateTimeOffset LastModified { get; set; }

        [JsonPropertyName("location")]
        public required string Location { get; set; }
    }
}
