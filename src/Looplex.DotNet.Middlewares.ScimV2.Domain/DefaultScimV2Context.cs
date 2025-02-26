using System.Dynamic;
using Looplex.OpenForExtension.Abstractions.Plugins;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain;

public class DefaultScimV2Context(IServiceProvider services) : IScimV2Context
{
    public bool SkipDefaultAction { get; set; } = false;

    public dynamic State { get; } = new ExpandoObject();

    public IDictionary<string, object> Roles { get; } = new Dictionary<string, object>();

    public IList<IPlugin> Plugins { get; } = [];

    public IServiceProvider Services { get; } = services;

    public object? Result { get; set; }
    
    public Dictionary<string, object?> RouteValues { get; set; } = [];

    public Dictionary<string, string> Query { get; set; } = [];

    public Dictionary<string, string> Headers { get; set; } = [];
}