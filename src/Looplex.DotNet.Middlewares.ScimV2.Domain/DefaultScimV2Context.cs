using System.Dynamic;
using Looplex.OpenForExtension.Abstractions.Plugins;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain;

public class DefaultScimV2Context : IScimV2Context
{
    public bool SkipDefaultAction { get; set; } = false;

    public object State { get; } = new ExpandoObject();

    public IDictionary<string, object> Roles { get; } = new Dictionary<string, object>();

    public IList<IPlugin> Plugins { get; private init; } = [];

    public IServiceProvider Services { get; private init; } = null!;

    public object? Result { get; set; }

    public Dictionary<string, object?> RouteValues { get; set; } = [];

    public Dictionary<string, string> Query { get; set; } = [];

    public Dictionary<string, string> Headers { get; set; } = [];
    
    public static IScimV2Context Create(IList<IPlugin> plugins, IServiceProvider services)
    {
        return new DefaultScimV2Context()
        {
            Plugins = plugins,
            Services = services
        };
    }
}