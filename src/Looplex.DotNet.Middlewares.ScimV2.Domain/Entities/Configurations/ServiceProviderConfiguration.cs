using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

/// <summary>
/// The service provider configuration resource enables a service provider to discover SCIM
/// specification features in a standardized form as well as provide additional
/// implementation details to clients. All attributes have a mutability of `readOnly`.
/// Unlike other core resources, the `id` attribute is not required for the service provider
/// configuration resource.
/// </summary>
public partial class ServiceProviderConfiguration
{
    public virtual IList<ResourceMap> Map { get; private set; } = new List<ResourceMap>();
    
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
    public static string ToJson(this ServiceProviderConfiguration self)
    {
        return JsonConvert.SerializeObject(self, ServiceProviderConfiguration.Converter.Settings);
    }
}

public class ResourceMap
{
    public required Type Type { get; init; }
    public required string Resource { get; init; }
    public required Type Service { get; init; }
}