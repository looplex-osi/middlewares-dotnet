using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

public partial class ResourceType
{
    /// <summary>
    /// A description of the authentication scheme.
    /// </summary>
    [JsonProperty("schemas")]
    public required string[] Schemas { get; init; }
    
    /// <summary>
    /// The resource type's server unique id.  This is often the same value as the "name" attribute.
    /// </summary>
    [JsonProperty("id")]
    public required string Id { get; init; }
    
    /// <summary>
    /// The resource type name.  When applicable, service providers MUST specify the name, e.g., "User" or "Group".  This name is
    /// referenced by the "meta.resourceType" attribute in all resources.
    /// </summary>
    [JsonProperty("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The resource type's human-readable description.  When applicable, service providers MUST specify the description.
    /// </summary>
    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
    public string? Description { get; set; }
    
    /// <summary>
    /// The resource type's HTTP-addressable endpoint relative to the Base URL of the service provider, e.g., "Users". 
    /// </summary>
    [JsonProperty("endpoint")]
    public required string Endpoint { get; init; }
    
    /// <summary>
    /// The resource type's primary/base schema URI, e.g., "urn:ietf:params:scim:schemas:core:2.0:User".  This MUST be equal
    /// to the "id" attribute of the associated "Schema" resource.
    /// </summary>
    [JsonProperty("schema")]
    public required string Schema { get; init; }
    
    [JsonProperty("meta", NullValueHandling = NullValueHandling.Ignore)]
    public virtual ResourceTypeMeta? Meta { get; set; }
}