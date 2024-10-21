namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities;

public class ServerProviderConfiguration
{
    public List<IResourceTypeConfiguration> ResourceTypes = [];
}

public interface IResourceTypeConfiguration;

public class ResourceTypeConfiguration<T> : IResourceTypeConfiguration where T : Resource
{
    public required T ResourceType { init; get; }
    public required string JsonSchema { init; get; }
    public required string Name { init; get; }
}