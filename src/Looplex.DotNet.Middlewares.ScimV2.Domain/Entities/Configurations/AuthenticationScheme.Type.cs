using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

public partial class AuthenticationScheme
{
    /// <summary>
    /// A description of the authentication scheme.
    /// </summary>
    [JsonProperty("description")]
    public required string Description { get; set; }

    /// <summary>
    /// An HTTP-addressable URL pointing to the authentication scheme's usage documentation.
    /// </summary>
    [JsonProperty("documentationUri", NullValueHandling = NullValueHandling.Ignore)]
    public Uri? DocumentationUri { get; set; }

    /// <summary>
    /// The common authentication scheme name.
    /// </summary>
    [JsonProperty("name")]
    public required string Name { get; set; }

    /// <summary>
    /// An HTTP-addressable URL pointing to the authentication scheme's specification.
    /// </summary>
    [JsonProperty("specUri", NullValueHandling = NullValueHandling.Ignore)]
    public Uri? SpecUri { get; set; }

    /// <summary>
    /// The authentication scheme.
    /// </summary>
    [JsonProperty("type")]
    public AuthenticationSchemeType Type { get; set; }
}