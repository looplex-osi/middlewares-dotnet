using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;

public partial class BulkRequest
{
    /// <summary>
    /// The number of errors that the service provider will accept before the operation is
    /// terminated. OPTIONAL in a request.
    /// </summary>
    [JsonProperty("failOnErrors", NullValueHandling = NullValueHandling.Ignore)]
    public long? FailOnErrors { get; set; }

    /// <summary>
    /// Defines operations within a bulk job. Each operation corresponds to a single HTTP request
    /// against a resource endpoint.
    /// </summary>
    [JsonProperty("Operations")]
    public List<BulkRequestOperation> Operations { get; set; } = [];
}