using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain;

public partial interface IScimV2Context : IContext
{
    Dictionary<string, object?> RouteValues { get; set; }
    Dictionary<string, string> Query { get; set; }
    Dictionary<string, string> Headers { get; set; }
    string GetDomain();
    /// <summary>
    /// The SqlDatabaseService instance is based on the domain value for the operation.
    /// It should be already resolved in the context operation.
    /// It's equivalent of calling ISqlDatabaseProvider.GetDatabaseAsync(domain).
    /// </summary>
    Task<ISqlDatabaseService> GetSqlDatabaseService();
}