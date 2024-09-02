using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Abstractions;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;

public partial class Meta : ObservableType
{
    public override IList<string> ChangedProperties { get; } = new List<string>();
    public override IDictionary<string, IList<object>> AddedItems { get; } = new Dictionary<string, IList<object>>();
    public override IDictionary<string, IList<object>> RemovedItems { get; } = new Dictionary<string, IList<object>>();
}