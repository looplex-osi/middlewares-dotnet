using Looplex.OpenForExtension.Context;
using Microsoft.AspNetCore.Http;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public class ScimV2RouteOptions
{
    public string[] ServicesForGet { get; init; } = []; 
    public Action<IDefaultContext, HttpContext>? CustomActionForGet { get; init; }
    
    public string[] ServicesForGetById { get; init; } = []; 
    public Action<IDefaultContext, HttpContext>? CustomActionForGetById { get; init; }
    
    public string[] ServicesForPost { get; init; } = [];
    public Action<IDefaultContext, HttpContext>? CustomActionForPost { get; init; }
    
    public string[] ServicesForDelete { get; init; } = []; 
    public Action<IDefaultContext, HttpContext>? CustomActionForDelete { get; init; }
}