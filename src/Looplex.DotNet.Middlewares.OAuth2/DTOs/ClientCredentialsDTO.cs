using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.OAuth2.DTOs
{
    public class ClientCredentialsDTO
    {
        [JsonPropertyName("grant_type")]
        public required string GrantType { get; init; }

        [JsonPropertyName("id_token")]
        public required string IdToken { get; init; }
    }
}
