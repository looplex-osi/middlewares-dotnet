using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.OpenForExtension.Context;

namespace Looplex.DotNet.Middlewares.OAuth2.Services
{
    public interface IClientService : ICrudService
    {
        Task GetByIdAndSecretOrDefaultAsync(IDefaultContext context);
    }
}
