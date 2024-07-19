namespace Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;

public class ScimV2RouteOptions
{
    public RouteOptions OptionsForGet { get; init; } = new();
    public RouteOptions OptionsForGetById { get; init; } = new();
    public RouteOptions OptionsForPost { get; init; } = new();
    public RouteOptions OptionsForDelete { get; init; } = new();
}