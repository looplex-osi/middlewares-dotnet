using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.OpenForExtensions;

public delegate Task ExtensionPointAsyncDelegate(IContext context, CancellationToken cancellationToken);

public interface IExtensionPointOrchestrator
{
    Task OrchestrateAsync(
        IContext context,
        ExtensionPointAsyncDelegate handleInputFunc,
        ExtensionPointAsyncDelegate validateInputFunc,
        ExtensionPointAsyncDelegate defineRolesFunc,
        ExtensionPointAsyncDelegate bindFunc,
        ExtensionPointAsyncDelegate beforeActionFunc,
        ExtensionPointAsyncDelegate defaultActionFunc,
        ExtensionPointAsyncDelegate afterActionFunc,
        ExtensionPointAsyncDelegate releaseUnmanagedFunc,
        CancellationToken cancellationToken);
}