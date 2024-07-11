using Looplex.OpenForExtension.Context;
using Microsoft.AspNetCore.Http;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public class ScimV2RouteOptions
{
    public string[] Services { get; init; } = []; 
    public Action<IDefaultContext, HttpContext>? GetCustomAction { get; init; }
    public Action<IDefaultContext, HttpContext>? GetByIdCustomAction { get; init; }
    public Action<IDefaultContext, HttpContext>? PostCustomAction { get; init; }
    public Action<IDefaultContext, HttpContext>? DeleteCustomAction { get; init; }
}