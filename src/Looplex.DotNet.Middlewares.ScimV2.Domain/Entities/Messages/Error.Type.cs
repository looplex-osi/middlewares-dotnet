using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;

public partial class Error : Exception
{
    [JsonProperty("detail")]
    public string Detail { get; private set; }

    [JsonProperty("scimType")]
    public ErrorScimType ScimType { get; private set; }

    [JsonProperty("status")]
    public string Status { get; private set; }
}