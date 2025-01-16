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
    
    public virtual Task GetAllAsync(IContext context)
    {
        rbacService.ThrowIfUnauthorized(context, GetType().Name, this.GetCallerName());
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            GetAllHandleInputAsync,
            GetAllValidateInputAsync,
            GetAllDefineRolesAsync,
            GetAllBindAsync,
            GetAllBeforeActionAsync,
            GetAllDefaultActionAsync,
            GetAllAfterActionAsync,
            GetAllReleaseUnmanagedResourcesAsync);
    }
    
    protected abstract Task GetAllHandleInputAsync(IContext context);
    protected abstract Task GetAllValidateInputAsync(IContext context);
    protected abstract Task GetAllDefineRolesAsync(IContext context);
    protected abstract Task GetAllBindAsync(IContext context);
    protected abstract Task GetAllBeforeActionAsync(IContext context);
    protected abstract Task GetAllDefaultActionAsync(IContext context);
    protected abstract Task GetAllAfterActionAsync(IContext context);
    protected abstract Task GetAllReleaseUnmanagedResourcesAsync(IContext context);
    
    #endregion
    
    #region GetById
    
    public virtual Task GetByIdAsync(IContext context)
    {
        rbacService.ThrowIfUnauthorized(context, GetType().Name, this.GetCallerName());
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            GetByIdHandleInputAsync,
            GetByIdValidateInputAsync,
            GetByIdDefineRolesAsync,
            GetByIdBindAsync,
            GetByIdBeforeActionAsync,
            GetByIdDefaultActionAsync,
            GetByIdAfterActionAsync,
            GetByIdReleaseUnmanagedResourcesAsync);
    }

    protected abstract Task GetByIdHandleInputAsync(IContext context);
    protected abstract Task GetByIdValidateInputAsync(IContext context);
    protected abstract Task GetByIdDefineRolesAsync(IContext context);
    protected abstract Task GetByIdBindAsync(IContext context);
    protected abstract Task GetByIdBeforeActionAsync(IContext context);
    protected abstract Task GetByIdDefaultActionAsync(IContext context);
    protected abstract Task GetByIdAfterActionAsync(IContext context);
    protected abstract Task GetByIdReleaseUnmanagedResourcesAsync(IContext context);
    
    #endregion
    
    #region Create
    
    public virtual Task CreateAsync(IContext context)
    {
        rbacService.ThrowIfUnauthorized(context, GetType().Name, this.GetCallerName());
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            CreateHandleInputAsync,
            CreateValidateInputAsync,
            CreateDefineRolesAsync,
            CreateBindAsync,
            CreateBeforeActionAsync,
            CreateDefaultActionAsync,
            CreateAfterActionAsync,
            CreateReleaseUnmanagedResourcesAsync);
    }

    protected abstract Task CreateHandleInputAsync(IContext context);
    protected abstract Task CreateValidateInputAsync(IContext context);
    protected abstract Task CreateDefineRolesAsync(IContext context);
    protected abstract Task CreateBindAsync(IContext context);
    protected abstract Task CreateBeforeActionAsync(IContext context);
    protected abstract Task CreateDefaultActionAsync(IContext context);
    protected abstract Task CreateAfterActionAsync(IContext context);
    protected abstract Task CreateReleaseUnmanagedResourcesAsync(IContext context);
    
    #endregion
    
    #region Update
    
    public virtual Task UpdateAsync(IContext context)
    {
        rbacService.ThrowIfUnauthorized(context, GetType().Name, this.GetCallerName());
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            UpdateHandleInputAsync,
            UpdateValidateInputAsync,
            UpdateDefineRolesAsync,
            UpdateBindAsync,
            UpdateBeforeActionAsync,
            UpdateDefaultActionAsync,
            UpdateAfterActionAsync,
            UpdateReleaseUnmanagedResourcesAsync);
    }

    protected abstract Task UpdateHandleInputAsync(IContext context);
    protected abstract Task UpdateValidateInputAsync(IContext context);
    protected abstract Task UpdateDefineRolesAsync(IContext context);
    protected abstract Task UpdateBindAsync(IContext context);
    protected abstract Task UpdateBeforeActionAsync(IContext context);
    protected abstract Task UpdateDefaultActionAsync(IContext context);
    protected abstract Task UpdateAfterActionAsync(IContext context);
    protected abstract Task UpdateReleaseUnmanagedResourcesAsync(IContext context);
    
    #endregion
    
    #region Update
    
    public virtual Task PatchAsync(IContext context)
    {
        rbacService.ThrowIfUnauthorized(context, GetType().Name, this.GetCallerName());
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            PatchHandleInputAsync,
            PatchValidateInputAsync,
            PatchDefineRolesAsync,
            PatchBindAsync,
            PatchBeforeActionAsync,
            PatchDefaultActionAsync,
            PatchAfterActionAsync,
            PatchReleaseUnmanagedResourcesAsync);
    }

    protected abstract Task PatchHandleInputAsync(IContext context);
    protected abstract Task PatchValidateInputAsync(IContext context);
    protected abstract Task PatchDefineRolesAsync(IContext context);
    protected abstract Task PatchBindAsync(IContext context);
    protected abstract Task PatchBeforeActionAsync(IContext context);
    protected abstract Task PatchDefaultActionAsync(IContext context);
    protected abstract Task PatchAfterActionAsync(IContext context);
    protected abstract Task PatchReleaseUnmanagedResourcesAsync(IContext context);
    
    #endregion
    
    #region Delete
    
    public virtual Task DeleteAsync(IContext context)
    {
        rbacService.ThrowIfUnauthorized(context, GetType().Name, this.GetCallerName());
        
        return extensionPointOrchestrator.OrchestrateAsync(
            context,
            DeleteHandleInputAsync,
            DeleteValidateInputAsync,
            DeleteDefineRolesAsync,
            DeleteBindAsync,
            DeleteBeforeActionAsync,
            DeleteDefaultActionAsync,
            DeleteAfterActionAsync,
            DeleteReleaseUnmanagedResourcesAsync);
    }

    protected abstract Task DeleteHandleInputAsync(IContext context);
    protected abstract Task DeleteValidateInputAsync(IContext context);
    protected abstract Task DeleteDefineRolesAsync(IContext context);
    protected abstract Task DeleteBindAsync(IContext context);
    protected abstract Task DeleteBeforeActionAsync(IContext context);
    protected abstract Task DeleteDefaultActionAsync(IContext context);
    protected abstract Task DeleteAfterActionAsync(IContext context);
    protected abstract Task DeleteReleaseUnmanagedResourcesAsync(IContext context);
    
    #endregion
}