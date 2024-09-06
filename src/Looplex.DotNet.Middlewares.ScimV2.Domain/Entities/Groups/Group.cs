using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;

public partial class Group : Resource
{
    [JsonProperty("displayName")]
    public virtual string? DisplayName { get; set; }

    [JsonProperty("members")]
    public virtual IList<MemberElement> Members { get; set; } = new ObservableCollection<MemberElement>();
}