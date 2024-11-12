using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;

public partial class Error
{
    public Error()
    {
        Detail = string.Empty;
        Status = 0;
    }
    
    public Error(
        string detail,
        ErrorScimType scimType,
        int status,
        Exception? innerException = null) : base(detail, innerException)
    {
        Detail = detail;
        ScimType = scimType;
        Status = status;
    }
    
    public Error(
        string detail,
        int status,
        Exception? innerException = null) : base(detail, innerException)
    {
        Detail = detail;
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
    
    public static JToken ToJToken(this Error self)
    {
        return JToken.FromObject(self, JsonSerializer.Create(Error.Converter.Settings));
    }
}