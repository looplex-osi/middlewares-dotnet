using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;

public partial class Group : Resource
{
    private string? _displayName;
    private IList<MemberElement> _members = new ObservableCollection<MemberElement>();
    
    [JsonProperty("displayName")]
    public string? DisplayName 
    {
        get => _displayName;
        set
        {
            if (value != _displayName)
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("members")]
    public IList<MemberElement> Members
    {
        get => _members;
        set
        {
            _members = value;
            if (value is INotifyCollectionChanged collection)
                collection.CollectionChanged += OnCollectionChanged;
        }
    }
}