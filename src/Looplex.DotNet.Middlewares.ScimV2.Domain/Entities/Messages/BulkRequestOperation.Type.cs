using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;

public partial class BulkRequestOperation
{
    /// <summary>
    /// The transient identifier of a newly created resource. REQUIRED when 'method' is 'POST'.
    /// </summary>
    [JsonProperty("bulkId", NullValueHandling = NullValueHandling.Ignore)]
    public string? BulkId { get; set; }

    /// <summary>
    /// The resource data as it would appear for a single SCIM POST, PUT, or PATCH operation.
    /// REQUIRED when 'method' is 'POST', 'PUT', or 'PATCH'.
    /// </summary>
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public JToken? Data { get; set; }

    /// <summary>
    /// The HTTP method of the current operation.
    /// </summary>
    [JsonProperty("method")]
    public Method Method { get; set; }

    /// <summary>
    /// The resource's relative path. REQUIRED in a request.
    /// </summary>
    [JsonProperty("path")]
    public required string Path { get; set; }

    /// <summary>
    /// The current resource version. Used if the service provider supports ETags and 'method' is
    /// 'PUT', 'PATCH', or 'DELETE'.
    /// </summary>
    [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
    public string? Version { get; set; }
}