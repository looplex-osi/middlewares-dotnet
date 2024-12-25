using Looplex.OpenForExtension.Abstractions.Contexts;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;

public static class ContextExtensionMethods
{
    public static IScimV2Context AsScimV2Context(this IContext context)
    {
        if (context is IScimV2Context scimV2Context)
            return scimV2Context;
        throw new InvalidCastException($"Param {nameof(context)} is not of type {nameof(IScimV2Context)}");
    }
    
    public static T GetRouteValue<T>(this IContext context, string key)
    {
        var scimV2Context = context.AsScimV2Context();
        var value = scimV2Context.RouteValues[key];
        if (value is T castValue)
            return castValue; 
        throw new InvalidCastException($"Context route value {key} is not of type {nameof(T)}");
    }
    
    public static T GetRequiredRouteValue<T>(this IContext context, string key)
    {
        var value = context.GetRouteValue<T>(key);
        if (value == null)
            throw new ArgumentNullException($"Context route value for {key} is null");
        return value;
    }
    
    public static string? GetQuery(this IContext context, string key)
    {
        var scimV2Context = context.AsScimV2Context();
        return scimV2Context.Query.GetValueOrDefault(key);
    }
    
    public static string GetRequiredQuery(this IContext context, string key)
    {
        var value = context.GetQuery(key);
        if (value == null)
            throw new ArgumentNullException($"Context route value for {key} is null");
        return value;
    }
    
    public static string? GetHeader(this IContext context, string key)
    {
        var scimV2Context = context.AsScimV2Context();
        return scimV2Context.Headers.GetValueOrDefault(key);
    }
    
    public static string GetRequiredHeader(this IContext context, string key)
    {
        var value = context.GetHeader(key);
        if (value == null)
            throw new ArgumentNullException($"Context route value for {key} is null");
        return value;
    }
}