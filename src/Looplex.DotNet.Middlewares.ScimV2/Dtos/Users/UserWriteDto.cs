using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.ScimV2.Dtos.Users
{
    public class UserWriteDto : UserDto
    {
        [JsonPropertyName("name")]
        public required NameDto Name { get; set; }
    }
}
