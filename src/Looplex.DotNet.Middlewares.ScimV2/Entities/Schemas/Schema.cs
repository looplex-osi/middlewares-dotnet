namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Schemas;

public static partial class Schema
{
    public static readonly IDictionary<Type, string> Schemas = new Dictionary<Type, string>();

    public static void Add<T>(string schema) where T : Resource
    {
        Schemas.Add(typeof(T), schema);
    }
}