using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Looplex.OpenForExtension.Abstractions.Traits;
using Looplex.OpenForExtension.Traits;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Abstractions;

public abstract class ObservableType : IHasEventHandlerTrait
{
    public abstract IList<string> ChangedProperties { get; }
    public abstract IDictionary<string, IList<object>> AddedItems { get; }
    public abstract IDictionary<string, IList<object>> RemovedItems { get; }
    private readonly Dictionary<INotifyCollectionChanged, string> _collectionToPropertyNameMap = new();
    
    #region Events
    
    private static readonly string PropertyChangedEventName = "PropertyChanged";
    private static readonly string CollectionChangedEventName = "CollectionChanged";

    public IEventHandlingTrait EventHandling { get; } = new EventHandlingTrait([
        PropertyChangedEventName,
        CollectionChangedEventName
    ]);

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        EventHandling.Invoke(PropertyChangedEventName, this, new PropertyChangedEventArgs(propertyName));
        if (!ChangedProperties.Contains(propertyName))
            ChangedProperties.Add(propertyName);
    }

    protected void BindOnCollectionChanged(ref INotifyCollectionChanged collection,
        [CallerMemberName] string propertyName = "")
    {
        if (GetType().GetProperty(propertyName) == null)
            throw new MissingMemberException(propertyName);
        
        collection.CollectionChanged += OnCollectionChanged;
        _collectionToPropertyNameMap[collection] = propertyName;
    }
    
    protected virtual void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var propertyName = _collectionToPropertyNameMap[(INotifyCollectionChanged)sender!];
        EventHandling.Invoke(CollectionChangedEventName, this, e);
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            if (!AddedItems.ContainsKey(propertyName))
                AddedItems.Add(propertyName, new List<object>());
            foreach (var item in e.NewItems!)
            {
                AddedItems[propertyName].Add(item);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (!RemovedItems.ContainsKey(propertyName))
                RemovedItems.Add(propertyName, new List<object>());
            foreach (var item in e.OldItems!)
            {
                RemovedItems[propertyName].Add(item);
            }
        }
        else throw new InvalidOperationException($"Cannot perform {e.Action} action on {propertyName}");
    }
    
    public void On(string eventName, EventHandler eventHandler) =>
        EventHandling.On(eventName, eventHandler);
    
    #endregion
}