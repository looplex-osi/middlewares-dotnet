using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;

public partial class Group
{
    #region Serialization
    
    public static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            }
        };
    }
    
    #endregion
}

public static class Serialize
{
    public static string ToJson(this Group self)
    {
        return JsonConvert.SerializeObject(self, Group.Converter.Settings);
    }
}