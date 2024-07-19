using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class EmailElement
{
    internal class EmailTypeConverter : JsonConverter
    {
        public static readonly EmailTypeConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return t == typeof(EmailType) || t == typeof(EmailType?);
        }

        public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "home":
                    return EmailType.Home;
                case "other":
                    return EmailType.Other;
                case "work":
                    return EmailType.Work;
            }

            throw new Exception("Cannot unmarshal type EmailType");
        }

        public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (EmailType)untypedValue;
            switch (value)
            {
                case EmailType.Home:
                    serializer.Serialize(writer, "home");
                    return;
                case EmailType.Other:
                    serializer.Serialize(writer, "other");
                    return;
                case EmailType.Work:
                    serializer.Serialize(writer, "work");
                    return;
            }

            throw new Exception("Cannot marshal type EmailType");
        }
    }
}