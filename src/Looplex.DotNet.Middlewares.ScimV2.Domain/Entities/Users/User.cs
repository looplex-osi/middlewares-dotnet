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
public partial class User
{
    public static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                AddressElement.AddressTypeConverter.Singleton,
                EmailElement.EmailTypeConverter.Singleton,
                GroupElement.GroupTypeConverter.Singleton,
                PhoneNumberElement.PhoneNumberTypeConverter.Singleton,
                PhotoElement.PhotoTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            }
        };
    }
}

public static class Serialize
{
    public static string ToJson(this User self)
    {
        return JsonConvert.SerializeObject(self, User.Converter.Settings);
    }
}