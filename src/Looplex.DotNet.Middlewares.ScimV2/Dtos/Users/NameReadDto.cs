using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.Dtos.Users
{
    public class NameReadDto : NameDto
    {
        [JsonPropertyName("formatted")]
        public required string Formatted { get; set; }
    }
}
