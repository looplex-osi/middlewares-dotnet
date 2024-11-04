using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;

public interface IBulkService
{
    Task ExecuteBulkOperationsAsync(IContext context, CancellationToken cancellationToken);
}