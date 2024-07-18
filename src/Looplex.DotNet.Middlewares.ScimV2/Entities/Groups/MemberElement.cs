using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Groups;

public partial class MemberElement
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
                case "group":
                    return GroupType.Group;
                case "user":
                    return GroupType.User;
            }

            throw new Exception("Cannot unmarshal type TypeEnum");
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
                case GroupType.Group:
                    serializer.Serialize(writer, "group");
                    return;
                case GroupType.User:
                    serializer.Serialize(writer, "user");
                    return;
            }

            throw new Exception("Cannot marshal type TypeEnum");
        }
    }
}