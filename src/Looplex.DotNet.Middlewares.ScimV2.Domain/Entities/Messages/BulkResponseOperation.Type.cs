using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;

public partial class BulkResponseOperation
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
    public object? Data { get; set; }

    /// <summary>
    /// The resource endpoint URL. REQUIRED in a response, except in the event of a POST failure.
    /// </summary>
    [JsonProperty("location")]
    public Uri? Location { get; set; }

    /// <summary>
    /// The HTTP method of the current operation.
    /// </summary>
    [JsonProperty("method")]
    public Method Method { get; set; }

    /// <summary>
    /// The resource's relative path. REQUIRED in a request.
    /// </summary>
    [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
    public string? Path { get; set; }

    /// <summary>
    /// The HTTP response body for the specified request operation. MUST be included when
    /// indicating an HTTP status other than 200.
    /// </summary>
    [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
    public object? Response { get; set; }

    /// <summary>
    /// The HTTP response status code for the requested operation.
    /// </summary>
    [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
    public long? Status { get; set; }

    /// <summary>
    /// The current resource version. Used if the service provider supports ETags and 'method' is
    /// 'PUT', 'PATCH', or 'DELETE'.
    /// </summary>
    [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
    public string? Version { get; set; }
}