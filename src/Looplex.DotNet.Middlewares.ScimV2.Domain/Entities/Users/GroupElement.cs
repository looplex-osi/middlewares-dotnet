using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class GroupElement
{
    internal class GroupTypeConverter : JsonConverter
    {
        public static readonly GroupTypeConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return t == typeof(GroupType) || t == typeof(GroupType?);
        }

        public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "direct":
                    return GroupType.Direct;
                case "indirect":
                    return GroupType.Indirect;
            }

            throw new Exception("Cannot unmarshal type GroupType");
        }

        public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (GroupType)untypedValue;
            switch (value)
            {
                case GroupType.Direct:
                    serializer.Serialize(writer, "direct");
                    return;
                case GroupType.Indirect:
                    serializer.Serialize(writer, "indirect");
                    return;
            }

            throw new Exception("Cannot marshal type GroupType");
        }
    }
}