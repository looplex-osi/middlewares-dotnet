using Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.Clients.Domain.Entities.Clients;

public partial class Client : Resource, IClient
{
    [JsonProperty("displayName")]
    public virtual required string DisplayName { get; set; }

    [JsonProperty("expirationTime")]
    public virtual DateTimeOffset ExpirationTime { get; set; }

    [JsonProperty("notBefore")]
    public virtual DateTimeOffset NotBefore { get; set; }

    [JsonProperty("secret")]
    public virtual required string Secret { get; set; }
}