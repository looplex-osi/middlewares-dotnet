using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain;

public partial interface IScimV2Context : IContext
{
    Dictionary<string, object?> RouteValues { get; set; }
    Dictionary<string, string> Query { get; set; }
    Dictionary<string, string> Headers { get; set; }
}