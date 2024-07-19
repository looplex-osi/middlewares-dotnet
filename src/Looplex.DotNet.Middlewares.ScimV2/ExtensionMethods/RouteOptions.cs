using Looplex.OpenForExtension.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public class RouteOptions
{
    public string[] Services { get; init; } = []; 
    public Action<IDefaultContext, HttpContext>? CustomAction { get; init; }
    public Action<RouteHandlerBuilder>? AfterMapAction { get; init; }
}