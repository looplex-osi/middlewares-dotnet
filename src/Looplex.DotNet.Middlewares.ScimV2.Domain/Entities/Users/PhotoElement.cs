using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class PhotoElement
{
    internal class PhotoTypeConverter : JsonConverter
    {
        public static readonly PhotoTypeConverter Singleton = new();

        public override bool CanConvert(Type t)
        {
            return t == typeof(PhotoType) || t == typeof(PhotoType?);
        }

        public override object? ReadJson(JsonReader reader, Type t, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "photo":
                    return PhotoType.Photo;
                case "thumbnail":
                    return PhotoType.Thumbnail;
            }

            throw new Exception("Cannot unmarshal type PhotoType");
        }

        public override void WriteJson(JsonWriter writer, object? untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }

            var value = (PhotoType)untypedValue;
            switch (value)
            {
                case PhotoType.Photo:
                    serializer.Serialize(writer, "photo");
                    return;
                case PhotoType.Thumbnail:
                    serializer.Serialize(writer, "thumbnail");
                    return;
            }

            throw new Exception("Cannot marshal type PhotoType");
        }
    }
}