using System.Dynamic;
using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Providers;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.OpenForExtension.Abstractions.Plugins;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain;

public class DefaultScimV2Context(
    IServiceProvider services,
    ISqlDatabaseProvider sqlDatabaseProvider) : IScimV2Context
{
    const string LooplexTenantKeyHeader = "X-looplex-tenant";

    public bool SkipDefaultAction { get; set; } = false;

    public object State { get; } = new ExpandoObject();

    public IDictionary<string, object> Roles { get; } = new Dictionary<string, object>();

    public IList<IPlugin> Plugins { get; } = [];

    public IServiceProvider Services { get; } = services;

    public object? Result { get; set; }

    public Dictionary<string, object?> RouteValues { get; set; } = [];

    public Dictionary<string, string> Query { get; set; } = [];

    public Dictionary<string, string> Headers { get; set; } = [];

    private ISqlDatabaseService? _sqlDatabaseService;


    public string GetDomain()
    {
        if (!Headers.TryGetValue(LooplexTenantKeyHeader, out var domain))
            throw new Error(
                $"{LooplexTenantKeyHeader} not found in context header.",
                (int)HttpStatusCode.BadRequest);
        if (string.IsNullOrWhiteSpace(domain))
            throw new Error(
                $"Domain should not be null or empty.",
                (int)HttpStatusCode.BadRequest);

        return domain;
    }

    public async Task<ISqlDatabaseService> GetSqlDatabaseService()
    {
        if (_sqlDatabaseService == null)
        {
            var domain = GetDomain();
            _sqlDatabaseService = await sqlDatabaseProvider.GetDatabaseAsync(domain);
        }

        return _sqlDatabaseService;
    }
}