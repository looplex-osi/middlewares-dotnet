using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Schemas;
using Looplex.OpenForExtension.Abstractions.Traits;
using Looplex.OpenForExtension.Traits;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;

public abstract partial class Resource : IHasEventHandlerTrait
{
    #region Serialization
    
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
    
    #endregion
    
    #region Events
    
    private static readonly string PropertyChangedEventName = "PropertyChanged";
    private static readonly string CollectionChangedEventName = "CollectionChanged";

    public IEventHandlingTrait EventHandling { get; } = new EventHandlingTrait([PropertyChangedEventName]);

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        EventHandling.Invoke(PropertyChangedEventName, this, new PropertyChangedEventArgs(propertyName));
    }
    
    protected virtual void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        EventHandling.Invoke(CollectionChangedEventName, this, e);
    }
    
    public void On(string eventName, EventHandler eventHandler) =>
        EventHandling.On(eventName, eventHandler);
    
    #endregion
}