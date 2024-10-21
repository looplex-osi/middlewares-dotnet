namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Configurations;

public class ResourceTypeConfiguration
{
    public required string Name { init; get; }
    public required Type ResourceType { init; get; }
    public required string JsonSchema { init; get; }
}