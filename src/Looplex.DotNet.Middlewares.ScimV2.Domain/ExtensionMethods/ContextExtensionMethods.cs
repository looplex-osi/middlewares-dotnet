using System.Net;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.OpenForExtension.Abstractions.Contexts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;

public static class ContextExtensionMethods
{
    public static IScimV2Context AsScimV2Context(this IContext context)
    {
        if (context is IScimV2Context scimV2Context)
            return scimV2Context;
        throw new InvalidCastException($"Param {nameof(context)} is not of type {nameof(IScimV2Context)}");
    }
    
    public static void DisposeIfPossible(this IContext context)
    {
        if (context is IDisposable disposableContext)
            disposableContext.Dispose();
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

    /// <summary>
    /// Applies attribute mapping (excludeAttributes) to the result object if it is a JObject
    /// or a string representation of a json object.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="Error">attributes parameter not supported in this version</exception>
    public static dynamic? GetResult(this IContext context)
    {
        JObject? json = null;
        if (context.Result != null)
        {
            if (context.Result is string str)
                json = JObject.Parse(str);
            else if (context.Result is JObject token)
                json = token;
        }

        if (json != null)
        {
            if (context.AsScimV2Context().Query.ContainsKey("attributes"))
                throw new Error("Attributes query parameter not supported", (int)HttpStatusCode.NotImplemented);
            var excludedAttributes = (context.GetQuery("excludedAttributes") ?? "")
                .Split(",", StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var attribute in excludedAttributes)
            {
                var tokens = json.SelectTokens(attribute);
                foreach (var token in tokens)
                {
                    if (token is JValue)
                        token.Parent?.Remove();    
                    else 
                        token.Remove();
                }
            }
            
            if (context.Result is string)
                context.Result = JsonConvert.SerializeObject(json);
        }
        
        return context.Result;
    }
}