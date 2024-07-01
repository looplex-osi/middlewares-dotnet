using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.OAuth2.DTOs
{
    public class ClientReadDTO : ClientDTO
    {
        [JsonPropertyName("client_id")]
        public required string ClientId { get; init; }
    }
}
