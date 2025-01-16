using Looplex.DotNet.Core.Application.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.OpenForExtensions;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Looplex.OpenForExtension.Abstractions.Commands;
using Looplex.OpenForExtension.Abstractions.ExtensionMethods;

namespace Looplex.DotNet.Middlewares.ScimV2.Services;

public class DefaultExtensionPointOrchestrator : IExtensionPointOrchestrator
{
    public async Task OrchestrateAsync(
        IContext context,
        ExtensionPointAsyncDelegate handleInputFunc,
        ExtensionPointAsyncDelegate validateInputFunc,
        ExtensionPointAsyncDelegate defineRolesFunc,
        ExtensionPointAsyncDelegate bindFunc,
        ExtensionPointAsyncDelegate beforeActionFunc,
        ExtensionPointAsyncDelegate defaultActionFunc,
        ExtensionPointAsyncDelegate afterActionFunc,
        ExtensionPointAsyncDelegate releaseUnmanagedFunc)
    {
        var cancellationToken = context.GetRequiredValue<CancellationToken>("CancellationToken");
        cancellationToken.ThrowIfCancellationRequested();
        
        // enforce RBAC
        
        await handleInputFunc(context);
        await context.Plugins.ExecuteAsync<IHandleInput>(context, cancellationToken);

        await validateInputFunc(context);
        await context.Plugins.ExecuteAsync<IValidateInput>(context, cancellationToken);
         
        await defineRolesFunc(context);
        await context.Plugins.ExecuteAsync<IDefineRoles>(context, cancellationToken);

        await bindFunc(context);
        await context.Plugins.ExecuteAsync<IBind>(context, cancellationToken);

        await beforeActionFunc(context);
        await context.Plugins.ExecuteAsync<IBeforeAction>(context, cancellationToken);

        if (!context.SkipDefaultAction)
        {
            await defaultActionFunc(context);
        }

        await afterActionFunc(context);
        await context.Plugins.ExecuteAsync<IAfterAction>(context, cancellationToken);

        await releaseUnmanagedFunc(context);
        await context.Plugins.ExecuteAsync<IReleaseUnmanagedResources>(context, cancellationToken);
    }
}