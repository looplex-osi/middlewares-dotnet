using System.Dynamic;
using Looplex.DotNet.Core.Application.Abstractions.Providers;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.OpenForExtension.Abstractions.Plugins;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain;

public class DefaultScimV2Context(
    IServiceProvider services,
    ISqlDatabaseProvider sqlDatabaseProvider) : IScimV2Context, IDisposable
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
            _sqlDatabaseService.OpenConnection();
        }

        return _sqlDatabaseService;
    }

    public void Dispose()
    {
        _sqlDatabaseService?.Dispose();
    }
}