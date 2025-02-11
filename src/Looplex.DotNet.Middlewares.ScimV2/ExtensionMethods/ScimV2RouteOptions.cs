﻿using Looplex.DotNet.Core.WebAPI.Routes;

namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public class ScimV2RouteOptions
{
    public string? ResourceTypeName { get; init; }
    public string? ResourceTypeDescription { get; init; }
    public RouteBuilderOptions? OptionsForGet { get; init; }
    public RouteBuilderOptions? OptionsForGetById { get; init; }
    public RouteBuilderOptions? OptionsForPost { get; init; }
    public RouteBuilderOptions? OptionsForPut { get; init; }
    public RouteBuilderOptions? OptionsForPatch { get; init; } 
    public RouteBuilderOptions? OptionsForDelete { get; init; } 
}