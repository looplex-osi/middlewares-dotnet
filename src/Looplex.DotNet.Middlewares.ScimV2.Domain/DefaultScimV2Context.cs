using System.Dynamic;
using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Providers;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.OpenForExtension.Abstractions.Plugins;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain;

public class DefaultScimV2Context(
    IServiceProvider services,
    ISqlDatabaseProvider sqlDatabaseProvider) : IScimV2Context
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

    private ISqlDatabaseService? _sqlDatabaseService;

    public async Task<ISqlDatabaseService> GetSqlDatabaseService()
    {
        if (_sqlDatabaseService == null)
        {
            var domain = this.GetRequiredValue<string>("Tenant");
            _sqlDatabaseService = await sqlDatabaseProvider.GetDatabaseAsync(domain);
        }

        return _sqlDatabaseService;
    }
}