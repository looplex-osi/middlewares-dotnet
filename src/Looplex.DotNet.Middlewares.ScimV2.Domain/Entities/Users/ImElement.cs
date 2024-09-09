using Looplex.DotNet.Core.Domain.Traits;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

public partial class ImElement
{
    [JsonIgnore]
    public IChangedPropertyNotificationTrait ChangedPropertyNotification { get; } = new ChangedPropertyNotificationTrait();
}