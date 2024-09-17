using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Looplex.DotNet.Middlewares.ApiKeys.Domain.Entities.ApiKeys;

public partial class ApiKey
{
    #region Serialization

    public static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
    
    #endregion
}

public static class Serialize
{
    public static string ToJson(this ApiKey self) => JsonConvert.SerializeObject(self, ApiKey.Converter.Settings);
}