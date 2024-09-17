using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ApiKeys.Application.Abstractions.Services;

public interface IApiKeyService : ICrudService
{
    Task GetByIdAndSecretOrDefaultAsync(IContext context, CancellationToken cancellationToken);
}