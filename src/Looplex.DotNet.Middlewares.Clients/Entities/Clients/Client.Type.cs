using Looplex.DotNet.Middlewares.OAuth2.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Entities;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.Clients.Entities.Clients;

public partial class Client : Resource, IClient
{
    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    [JsonProperty("expirationTime")]
    public DateTimeOffset ExpirationTime { get; set; }

    [JsonProperty("notBefore")]
    public DateTimeOffset NotBefore { get; set; }

    [JsonProperty("secret")]
    public string Secret { get; set; }
}