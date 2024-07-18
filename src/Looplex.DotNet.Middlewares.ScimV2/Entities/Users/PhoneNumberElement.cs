using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

public partial class PhoneNumberElement
{
    internal class PhoneNumberTypeConverter : JsonConverter
    {
        public static readonly PhoneNumberTypeConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return t == typeof(PhoneNumberType) || t == typeof(PhoneNumberType?);
        }

        public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "fax":
                    return PhoneNumberType.Fax;
                case "home":
                    return PhoneNumberType.Home;
                case "mobile":
                    return PhoneNumberType.Mobile;
                case "other":
                    return PhoneNumberType.Other;
                case "pager":
                    return PhoneNumberType.Pager;
                case "work":
                    return PhoneNumberType.Work;
            }

            throw new Exception("Cannot unmarshal type PhoneNumberType");
        }

        public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (PhoneNumberType)untypedValue;
            switch (value)
            {
                case PhoneNumberType.Fax:
                    serializer.Serialize(writer, "fax");
                    return;
                case PhoneNumberType.Home:
                    serializer.Serialize(writer, "home");
                    return;
                case PhoneNumberType.Mobile:
                    serializer.Serialize(writer, "mobile");
                    return;
                case PhoneNumberType.Other:
                    serializer.Serialize(writer, "other");
                    return;
                case PhoneNumberType.Pager:
                    serializer.Serialize(writer, "pager");
                    return;
                case PhoneNumberType.Work:
                    serializer.Serialize(writer, "work");
                    return;
            }

            throw new Exception("Cannot marshal type PhoneNumberType");
        }
    }
}