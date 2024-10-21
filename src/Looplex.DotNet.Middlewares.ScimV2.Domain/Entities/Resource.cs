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
    
    public static T FromJson<T>(string json, out IList<string> messages)
    {
        JsonTextReader reader = new(new StringReader(json));

        JSchemaValidatingReader validatingReader = new(reader);
        validatingReader.Schema = JSchema.Parse(Schemas.Get(typeof(T)));

        IList<string> localMessages = [];
        validatingReader.ValidationEventHandler += (o, a) => localMessages.Add(a.Message);
        messages = localMessages;

        JsonSerializer serializer = new();
        return serializer.Deserialize<T>(validatingReader)!;
    }
    
    #endregion
}

/// <summary>
/// This class has a map of scimResource:jsonSchema and it is used for model validation on deserialization
/// in the scim resource's services 
/// </summary>
public static class Schemas
{
    internal static readonly IDictionary<Type, string> Map = new Dictionary<Type, string>();

    public static void Add(Type type, string schema)
    {
        Map.Add(type, schema);
    }
    
    public static bool ContainsKey(Type type)
    {
        return Map.ContainsKey(type);
    }
    
    public static string Get(Type type)
    {
        return Map[type];
    }
}