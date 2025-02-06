using Looplex.DotNet.Core.Domain.Traits;
using Looplex.OpenForExtension.Abstractions.Traits;
using Looplex.OpenForExtension.Traits;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;

public abstract partial class Resource
{
    protected Resource()
    {
        EventHandling = new EventHandlingTrait([
            ChangedPropertyNotification.PropertyChangedEventName,
            ChangedPropertyNotification.CollectionChangedEventName
        ]);
    }
    
    #region Events

    [JsonIgnore]
    public IChangedPropertyNotificationTrait ChangedPropertyNotification { get; } = new ChangedPropertyNotificationTrait();

    [JsonIgnore]
    public IEventHandlingTrait EventHandling { get; }
    
    public void On(string eventName, EventHandler eventHandler) =>
        EventHandling.On(eventName, eventHandler);
    
    #endregion
    
    #region Serialization
    
    public static T FromJson<T>(string json, string? jsonSchema, out IList<string> messages)
    {
        JsonTextReader reader = new(new StringReader(json));

        JSchemaValidatingReader validatingReader = new(reader);

        IList<string> localMessages = [];
        if (!string.IsNullOrWhiteSpace(jsonSchema))
        {
            validatingReader.Schema = JSchema.Parse(jsonSchema);
            validatingReader.ValidationEventHandler += (o, a) =>
            {
                localMessages.Add(a.Message);
                foreach (var childError in a.ValidationError.ChildErrors)
                    localMessages.Add(childError.Message);
            };
        }
        messages = localMessages;

        JsonSerializer serializer = new();
        return serializer.Deserialize<T>(validatingReader)!;
    }
    
    #endregion
}