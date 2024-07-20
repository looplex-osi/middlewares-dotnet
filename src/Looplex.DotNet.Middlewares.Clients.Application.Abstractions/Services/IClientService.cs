using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Middlewares.Clients.Application.Abstractions.Services;

public interface IClientService : ICrudService
{
    Task GetByIdAndSecretOrDefaultAsync(IDefaultContext context, CancellationToken cancellationToken);
}