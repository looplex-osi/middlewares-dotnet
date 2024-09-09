using Looplex.DotNet.Core.Domain.Traits;
using Newtonsoft.Json;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;

/// <summary>
///     The full name, including all middle names, titles, and suffixes as appropriate, formatted
///     for display (e.g., 'Ms. Barbara J Jensen, III').
/// </summary>
public partial class Name
{
    [JsonIgnore]
    public IChangedPropertyNotificationTrait ChangedPropertyNotification { get; } = new ChangedPropertyNotificationTrait();
}