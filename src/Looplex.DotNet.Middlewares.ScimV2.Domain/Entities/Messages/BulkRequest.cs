using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;

public partial class BulkRequest
{
    public static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new StringEnumConverter(),
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            }
        };
    }
}

public static partial class Serialize
{
    public static string ToJson(this BulkRequest self)
    {
        return JsonConvert.SerializeObject(self, BulkRequest.Converter.Settings);
    }
}