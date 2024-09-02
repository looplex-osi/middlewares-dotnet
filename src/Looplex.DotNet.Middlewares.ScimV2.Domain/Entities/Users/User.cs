using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

/// <summary>
///     SCIM provides a resource type for "User" resources.  The core schema
///     for "User" is identified using the following schema URI:
///     "urn:ietf:params:scim:schemas:core:2.0:User".
///     <see cref="https://datatracker.ietf.org/doc/html/rfc7643#section-4.1" />
/// </summary>
public partial class User : Resource
{
    public override IList<string> ChangedProperties { get; } = new List<string>();
    public override IDictionary<string, IList<object>> AddedItems { get; } = new Dictionary<string, IList<object>>();
    public override IDictionary<string, IList<object>> RemovedItems { get; } = new Dictionary<string, IList<object>>();
    
    #region Serialization
    
    public bool ShouldSerializePassword() => false;
    
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
    
    #endregion
}

public static class Serialize
{
    public static string ToJson(this User self)
    {
        return JsonConvert.SerializeObject(self, User.Converter.Settings);
    }
}