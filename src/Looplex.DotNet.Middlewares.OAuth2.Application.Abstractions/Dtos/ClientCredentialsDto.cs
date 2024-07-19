using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Dtos;

public class ClientCredentialsDto
{
    [JsonProperty("grant_type")]
    public required string GrantType { get; init; }

    [JsonProperty("id_token")]
    public required string IdToken { get; init; }
}