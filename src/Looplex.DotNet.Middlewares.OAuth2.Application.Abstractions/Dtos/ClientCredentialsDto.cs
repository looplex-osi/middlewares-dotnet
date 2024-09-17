using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Dtos;

public class ClientCredentialsDto
{
    [JsonProperty("grant_type")]
    public required string GrantType { get; init; }
    
    [JsonProperty("subject_token")]
    public string? SubjectToken { get; init; }
    
    [JsonProperty("subject_token_type")]
    public string? SubjectTokenType { get; init; }
}