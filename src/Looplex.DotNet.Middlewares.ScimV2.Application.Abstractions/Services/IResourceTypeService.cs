using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.Services;

public interface IResourceTypeService
{
    Task GetAllAsync(IContext context);
    Task GetByIdAsync(IContext context);
    Task CreateAsync(IContext context);
}