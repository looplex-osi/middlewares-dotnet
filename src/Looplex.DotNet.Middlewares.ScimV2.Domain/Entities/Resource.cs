using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Schemas;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;

public abstract partial class Resource
{
    public static T FromJson<T>(string json, out IList<string> messages)
    {
        JsonTextReader reader = new(new StringReader(json));

        JSchemaValidatingReader validatingReader = new(reader);
        validatingReader.Schema = JSchema.Parse(Schema.Schemas[typeof(T)]);

        IList<string> localMessages = [];
        validatingReader.ValidationEventHandler += (o, a) => localMessages.Add(a.Message);
        messages = localMessages;

        JsonSerializer serializer = new();
        return serializer.Deserialize<T>(validatingReader)!;
    }
}