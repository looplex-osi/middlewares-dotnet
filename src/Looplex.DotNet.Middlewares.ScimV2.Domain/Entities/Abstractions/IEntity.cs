namespace Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Abstractions;

public interface IEntity
{
    /// <summary>
    ///     Sequencial id for an entity.
    /// </summary>
    int? Id { get; set; }
}