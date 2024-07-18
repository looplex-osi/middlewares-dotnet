using System.Globalization;
using Looplex.DotNet.Middlewares.OAuth2.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Looplex.DotNet.Middlewares.Clients.Entities.Clients;

public partial class Client : Resource, IClient
{
    internal static class Converter
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
}

public static class Serialize
{
    public static string ToJson(this Client self) => JsonConvert.SerializeObject(self, Client.Converter.Settings);
}