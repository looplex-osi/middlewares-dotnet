using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Users;

public partial class AddressElement
{
    internal class AddressTypeConverter : JsonConverter
    {
        public static readonly AddressTypeConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return t == typeof(AddressType) || t == typeof(AddressType?);
        }

        public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "home":
                    return AddressType.Home;
                case "other":
                    return AddressType.Other;
                case "work":
                    return AddressType.Work;
            }

            throw new Exception("Cannot unmarshal type AddressType");
        }

        public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (AddressType)untypedValue;
            switch (value)
            {
                case AddressType.Home:
                    serializer.Serialize(writer, "home");
                    return;
                case AddressType.Other:
                    serializer.Serialize(writer, "other");
                    return;
                case AddressType.Work:
                    serializer.Serialize(writer, "work");
                    return;
            }

            throw new Exception("Cannot marshal type AddressType");
        }
    }
}