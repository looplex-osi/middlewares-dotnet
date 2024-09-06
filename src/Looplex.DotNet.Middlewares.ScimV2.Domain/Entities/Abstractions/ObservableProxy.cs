using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using Castle.DynamicProxy;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Abstractions;

public static class ObservableProxy
{
    private static readonly ProxyGenerator ProxyGenerator = new();
    
    public static T WithObservableProxy<T>(this T instance) where T : ObservableType, new()
    {
        var proxy = ProxyGenerator.CreateClassProxyWithTarget<T>(instance,new NotifyPropertyChangedInterceptor());
        
        var setters = typeof(T)
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.IsSpecialName && m.Name.StartsWith("set_"));
        
        foreach (var setter in setters)
        {
            var propertyName = setter.Name[4..];
            var property = typeof(T).GetProperty(propertyName)!;
            
            if (property.GetValue(proxy) is INotifyCollectionChanged collection)
            {
                BindOnCollectionChanged(property.Name, proxy, collection);
            }
        }

        return proxy;
    }

    private static void OnPropertyChanged(string propertyName, ObservableType observableType)
    {
        observableType.EventHandling.Invoke(ObservableType.PropertyChangedEventName, observableType,
            new PropertyChangedEventArgs(propertyName));
        if (!observableType.ChangedProperties.Contains(propertyName))
            observableType.ChangedProperties.Add(propertyName);
    }

    private static void BindOnCollectionChanged(string propertyName, ObservableType observableType,
        INotifyCollectionChanged collection)
    {
        collection.CollectionChanged += (_, e) =>
        {
            observableType.EventHandling.Invoke(ObservableType.CollectionChangedEventName, observableType, e);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (!observableType.AddedItems.ContainsKey(propertyName))
                    observableType.AddedItems.Add(propertyName, new List<object>());
                foreach (var item in e.NewItems!)
                {
                    observableType.AddedItems[propertyName].Add(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (!observableType.RemovedItems.ContainsKey(propertyName))
                    observableType.RemovedItems.Add(propertyName, new List<object>());
                foreach (var item in e.OldItems!)
                {
                    observableType.RemovedItems[propertyName].Add(item);
                }
            }
            else throw new InvalidOperationException($"Cannot perform {e.Action} action on {propertyName}");
        };
    }

    class NotifyPropertyChangedInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.IsSpecialName && invocation.Method.Name.StartsWith("set_"))
            {
                var propertyName = invocation.Method.Name[4..];

                var property = invocation.InvocationTarget.GetType().GetProperty(propertyName)!;
                var currentValue = property.GetValue(invocation.InvocationTarget)!;

                invocation.Proceed();

                var newValue = property.GetValue(invocation.InvocationTarget);

                if (!Equals(currentValue, newValue) && invocation.Proxy is ObservableType observableType)
                {
                    if (newValue is INotifyCollectionChanged newCollection)
                    {
                        BindOnCollectionChanged(propertyName, observableType, newCollection);
                    }
                    else
                    {
                        OnPropertyChanged(propertyName, observableType);
                    }
                }
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}