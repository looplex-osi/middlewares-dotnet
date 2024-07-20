using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.Clients.Application.Abstractions.Services;

public interface IClientService : ICrudService
{
    Task GetByIdAndSecretOrDefaultAsync(IContext context, CancellationToken cancellationToken);
}