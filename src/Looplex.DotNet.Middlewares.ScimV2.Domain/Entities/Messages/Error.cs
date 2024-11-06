using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;

public partial class Error
{
    public Error(
        string detail,
        ErrorScimType scimType,
        string status,
        Exception? innerException = null) : base(detail, innerException)
    {
        Detail = detail;
        ScimType = scimType;
        Status = status;
    }

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
    public static string ToJson(this Error self)
    {
        return JsonConvert.SerializeObject(self, Error.Converter.Settings);
    }
}