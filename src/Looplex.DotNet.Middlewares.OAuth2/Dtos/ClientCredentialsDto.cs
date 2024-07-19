using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.OAuth2.Dtos
{
    public class ClientCredentialsDto
    {
        [JsonPropertyName("grant_type")]
        public required string GrantType { get; init; }

        [JsonPropertyName("id_token")]
        public required string IdToken { get; init; }
    }
}
