using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Middlewares.ScimV2.Application.Abstractions.OpenForExtensions;
using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ScimV2.Services;

public abstract class BaseCrudService(
    IRbacService rbacService,
    IExtensionPointOrchestrator extensionPointOrchestrator) : ICrudService
{
    #region GetAll
    
    public virtual Task GetAllAsync(IContext context, CancellationToken cancellationToken)
    {
        var resource = $"{GetType().Name}.{this.GetCallerName()}";
        rbacService.ThrowIfUnauthorized(context, resource, "read", cancellationToken);
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            GetAllHandleInputAsync,
            GetAllValidateInputAsync,
            GetAllDefineRolesAsync,
            GetAllBindAsync,
            GetAllBeforeActionAsync,
            GetAllDefaultActionAsync,
            GetAllAfterActionAsync,
            GetAllReleaseUnmanagedResourcesAsync,
            cancellationToken);
    }
    
    protected abstract Task GetAllHandleInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetAllValidateInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetAllDefineRolesAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetAllBindAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetAllBeforeActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetAllDefaultActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetAllAfterActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetAllReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken);
    
    #endregion
    
    #region GetById
    
    public virtual Task GetByIdAsync(IContext context, CancellationToken cancellationToken)
    {
        var resource = $"{GetType().Name}.{this.GetCallerName()}";
        rbacService.ThrowIfUnauthorized(context, resource, "read", cancellationToken);
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            GetByIdHandleInputAsync,
            GetByIdValidateInputAsync,
            GetByIdDefineRolesAsync,
            GetByIdBindAsync,
            GetByIdBeforeActionAsync,
            GetByIdDefaultActionAsync,
            GetByIdAfterActionAsync,
            GetByIdReleaseUnmanagedResourcesAsync,
            cancellationToken);
    }

    protected abstract Task GetByIdHandleInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetByIdValidateInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetByIdDefineRolesAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetByIdBindAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetByIdBeforeActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetByIdDefaultActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetByIdAfterActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task GetByIdReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken);
    
    #endregion
    
    #region Create
    
    public virtual Task CreateAsync(IContext context, CancellationToken cancellationToken)
    {
        var resource = $"{GetType().Name}.{this.GetCallerName()}";
        rbacService.ThrowIfUnauthorized(context, resource, "write", cancellationToken);
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            CreateHandleInputAsync,
            CreateValidateInputAsync,
            CreateDefineRolesAsync,
            CreateBindAsync,
            CreateBeforeActionAsync,
            CreateDefaultActionAsync,
            CreateAfterActionAsync,
            CreateReleaseUnmanagedResourcesAsync,
            cancellationToken);
    }

    protected abstract Task CreateHandleInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task CreateValidateInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task CreateDefineRolesAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task CreateBindAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task CreateBeforeActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task CreateDefaultActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task CreateAfterActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task CreateReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken);
    
    #endregion
    
    #region Update
    
    public virtual Task UpdateAsync(IContext context, CancellationToken cancellationToken)
    {
        var resource = $"{GetType().Name}.{this.GetCallerName()}";
        rbacService.ThrowIfUnauthorized(context, resource, "write", cancellationToken);
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            UpdateHandleInputAsync,
            UpdateValidateInputAsync,
            UpdateDefineRolesAsync,
            UpdateBindAsync,
            UpdateBeforeActionAsync,
            UpdateDefaultActionAsync,
            UpdateAfterActionAsync,
            UpdateReleaseUnmanagedResourcesAsync,
            cancellationToken);
    }

    protected abstract Task UpdateHandleInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task UpdateValidateInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task UpdateDefineRolesAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task UpdateBindAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task UpdateBeforeActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task UpdateDefaultActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task UpdateAfterActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task UpdateReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken);
    
    #endregion
    
    #region Update
    
    public virtual Task PatchAsync(IContext context, CancellationToken cancellationToken)
    {
        var resource = $"{GetType().Name}.{this.GetCallerName()}";
        rbacService.ThrowIfUnauthorized(context, resource, "write", cancellationToken);
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            PatchHandleInputAsync,
            PatchValidateInputAsync,
            PatchDefineRolesAsync,
            PatchBindAsync,
            PatchBeforeActionAsync,
            PatchDefaultActionAsync,
            PatchAfterActionAsync,
            PatchReleaseUnmanagedResourcesAsync,
            cancellationToken);
    }

    protected abstract Task PatchHandleInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task PatchValidateInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task PatchDefineRolesAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task PatchBindAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task PatchBeforeActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task PatchDefaultActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task PatchAfterActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task PatchReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken);
    
    #endregion
    
    #region Delete
    
    public virtual Task DeleteAsync(IContext context, CancellationToken cancellationToken)
    {
        var resource = $"{GetType().Name}.{this.GetCallerName()}";
        rbacService.ThrowIfUnauthorized(context, resource, "delete", cancellationToken);
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            DeleteHandleInputAsync,
            DeleteValidateInputAsync,
            DeleteDefineRolesAsync,
            DeleteBindAsync,
            DeleteBeforeActionAsync,
            DeleteDefaultActionAsync,
            DeleteAfterActionAsync,
            DeleteReleaseUnmanagedResourcesAsync,
            cancellationToken);
    }

    protected abstract Task DeleteHandleInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task DeleteValidateInputAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task DeleteDefineRolesAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task DeleteBindAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task DeleteBeforeActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task DeleteDefaultActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task DeleteAfterActionAsync(IContext context, CancellationToken cancellationToken);
    protected abstract Task DeleteReleaseUnmanagedResourcesAsync(IContext context, CancellationToken cancellationToken);
    
    #endregion
}