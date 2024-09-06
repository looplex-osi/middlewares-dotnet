using Looplex.OpenForExtension.Abstractions.Traits;
using Looplex.OpenForExtension.Traits;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Abstractions;

public abstract class ObservableType : IHasEventHandlerTrait // TODO change to trait
{
    [JsonIgnore]
    public IList<string> ChangedProperties { get; } = new List<string>();
    [JsonIgnore]
    public IDictionary<string, IList<object>> AddedItems { get; } = new Dictionary<string, IList<object>>();
    [JsonIgnore]
    public IDictionary<string, IList<object>> RemovedItems { get; } = new Dictionary<string, IList<object>>();
    
    #region Events

    public const string PropertyChangedEventName = "PropertyChanged";
    public const string CollectionChangedEventName = "CollectionChanged";

    [JsonIgnore]
    public IEventHandlingTrait EventHandling { get; } = new EventHandlingTrait([
        PropertyChangedEventName,
        CollectionChangedEventName
    ]);
    
    public void On(string eventName, EventHandler eventHandler) =>
        EventHandling.On(eventName, eventHandler);
    
    #endregion
}