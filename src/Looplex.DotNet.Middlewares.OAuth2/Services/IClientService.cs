using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.OAuth2.Entities;

namespace Looplex.DotNet.Middlewares.OAuth2.Services
{
    public interface IClientService : ICrudService
    {
        Task<IClient?> GetByIdAndSecretOrDefaultAsync(Guid id, string secret);
    }
}
