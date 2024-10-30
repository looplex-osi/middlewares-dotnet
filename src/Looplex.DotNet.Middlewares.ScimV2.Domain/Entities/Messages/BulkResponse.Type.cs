using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;

public partial class BulkResponse
{
    /// <summary>
    /// Defines operations within a bulk job. Each operation corresponds to a single HTTP request
    /// against a resource endpoint.
    /// </summary>
    [JsonProperty("Operations")]
    public List<BulkResponseOperation> Operations { get; set; } = [];
}