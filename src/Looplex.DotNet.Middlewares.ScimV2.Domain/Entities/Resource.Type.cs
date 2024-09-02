using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;

public abstract partial class Resource
{
    /// <summary>
    ///     Sequencial id for an entity.
    /// </summary>
    [JsonIgnore]
    public int? Id { get; set; }

    /// <summary>
    ///     A unique identifier for an entity.
    /// </summary>
    [JsonProperty("id")]
    public Guid? UniqueId { get; set; }
    
    /// <summary>
    ///     A String that is an identifier for the resource as defined by the provisioning client.
    /// </summary>
    [JsonProperty("externalId", NullValueHandling = NullValueHandling.Ignore)]
    public string? ExternalId { get; set; }

    [JsonProperty("meta", NullValueHandling = NullValueHandling.Ignore)]
    public Meta? Meta { get; set; }
}