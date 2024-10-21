using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;

public interface ISchemaService
{
    Task GetAllAsync(IContext context, CancellationToken cancellationToken);
    Task GetByIdAsync(IContext context, CancellationToken cancellationToken);
    Task CreateAsync(IContext context, CancellationToken cancellationToken);
}