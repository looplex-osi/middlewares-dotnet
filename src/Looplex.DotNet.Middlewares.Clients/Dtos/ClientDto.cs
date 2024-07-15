using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.Clients.Dtos
{
    public class ClientDto
    {
        [JsonPropertyName("display_name")]
        public required string DisplayName { get; init; }

        [JsonPropertyName("secret")]
        public required string Secret { get; init; }

        [JsonPropertyName("expiration_time")]
        public required DateTime ExpirationTime { get; init; }

        [JsonPropertyName("not_before")]
        public required DateTime NotBefore { get; init; }
    }
}
