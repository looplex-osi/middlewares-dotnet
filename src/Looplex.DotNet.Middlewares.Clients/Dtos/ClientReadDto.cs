using System.Text.Json.Serialization;

namespace Looplex.DotNet.Middlewares.Clients.Dtos
{
    public class ClientReadDto : ClientDto
    {
        [JsonPropertyName("client_id")]
        public required string Id { get; init; }
    }
}
