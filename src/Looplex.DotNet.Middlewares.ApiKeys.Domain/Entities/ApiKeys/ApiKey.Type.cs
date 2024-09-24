using Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ApiKeys.Domain.Entities.ApiKeys;

public partial class ApiKey : Resource, IApiKey
{
    [JsonProperty("clientName")]
    public virtual string? ClientName { get; set; }
    
    [JsonProperty("clientId")]
    public virtual Guid? ClientId { get; set; }

    [JsonIgnore]
    public string? Digest { get; set; }
    
    [JsonIgnore]
    public int? UserId { get; set; }

    [JsonProperty("expirationTime")]
    public virtual DateTimeOffset ExpirationTime { get; set; }

    [JsonProperty("notBefore")]
    public virtual DateTimeOffset NotBefore { get; set; }
}