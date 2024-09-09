using Looplex.DotNet.Core.Domain.Traits;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;

public partial class MemberElement
{
    [JsonIgnore]
    public IChangedPropertyNotificationTrait ChangedPropertyNotification { get; } = new ChangedPropertyNotificationTrait();
}