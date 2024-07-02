using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.Clients.DTOs
{
    public class ClientReadDTO : ClientDTO
    {
        [JsonPropertyName("client_id")]
        public required string Id { get; init; }
    }
}
