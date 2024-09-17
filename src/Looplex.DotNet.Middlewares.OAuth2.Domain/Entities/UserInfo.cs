using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;

public class UserInfo
{
    [JsonProperty("sub")]
    public virtual required string Sub { get; set; }
    
    [JsonProperty("name")]
    public virtual required string Name { get; set; }
    
    [JsonProperty("family_name")]
    public virtual required string FamilyName { get; set; }
    
    [JsonProperty("given_name")]
    public virtual required string GivenName { get; set; }
    
    [JsonProperty("picture")]
    public virtual required string Picture { get; set; }
    
    [JsonProperty("email")]
    public virtual required string Email { get; set; }
}