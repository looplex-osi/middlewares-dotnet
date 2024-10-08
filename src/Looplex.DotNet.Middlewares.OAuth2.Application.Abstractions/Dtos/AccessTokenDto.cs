﻿using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.OAuth2.Application.Abstractions.Dtos;

public class AccessTokenDto
{
    [JsonProperty("access_token")]
    public required string AccessToken { get; set; }
}