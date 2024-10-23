using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

/// <summary>
/// The service provider configuration resource enables a service provider to discover SCIM
/// specification features in a standardized form as well as provide additional
/// implementation details to clients. All attributes have a mutability of `readOnly`.
/// Unlike other core resources, the `id` attribute is not required for the service provider
/// configuration resource.
/// </summary>
public partial class ServiceProviderConfiguration
{
    /// <summary>
    /// A multi-valued complex type that specifies supported authentication scheme properties.
    /// To enable seamless discovery of configurations, the service provider SHOULD, with the
    /// appropriate security considerations, make the authenticationSchemes attribute publicly
    /// accessible without prior authentication.
    /// </summary>
    [JsonProperty("authenticationSchemes", NullValueHandling = NullValueHandling.Ignore)]
    public required AuthenticationScheme[] AuthenticationSchemes { get; set; } = [];

    /// <summary>
    /// A complex type that specifies bulk configuration options.  See Section 3.7 of [RFC7644].
    /// </summary>
    [JsonProperty("bulk")]
    public required Bulk Bulk { get; set; }

    /// <summary>
    /// A complex type that specifies configuration options related to changing a password.
    /// </summary>
    [JsonProperty("changePassword")]
    public required ChangePassword ChangePassword { get; set; }

    /// <summary>
    /// An HTTP-addressable URL pointing to the service provider`s human-consumable help
    /// documentation.
    /// </summary>
    [JsonProperty("documentationUri", NullValueHandling = NullValueHandling.Ignore)]
    public Uri? DocumentationUri { get; set; }

    /// <summary>
    /// A complex type that specifies ETag configuration options.
    /// </summary>
    [JsonProperty("etag")]
    public required Etag Etag { get; set; }

    /// <summary>
    /// A complex type that specifies FILTER options.
    /// </summary>
    [JsonProperty("filter")]
    public required Filter Filter { get; set; }

    /// <summary>
    /// A complex type that specifies PATCH configuration options.
    /// </summary>
    [JsonProperty("patch")]
    public required Patch Patch { get; set; }

    /// <summary>
    /// A complex type that specifies Sort configuration options.
    /// </summary>
    [JsonProperty("sort")]
    public required Sort Sort { get; set; }
}
