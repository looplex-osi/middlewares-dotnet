using System.Globalization;
using Looplex.DotNet.Middlewares.OAuth2.Domain.Entities;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Looplex.DotNet.Middlewares.Clients.Domain.Entities.Clients;

public partial class Client : Resource, IClient
{
    public override IList<string> ChangedProperties { get; } = new List<string>();
    public override IDictionary<string, IList<object>> AddedItems { get; } = new Dictionary<string, IList<object>>();
    public override IDictionary<string, IList<object>> RemovedItems { get; } = new Dictionary<string, IList<object>>();
    
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
    public static string ToJson(this Client self) => JsonConvert.SerializeObject(self, Client.Converter.Settings);
}