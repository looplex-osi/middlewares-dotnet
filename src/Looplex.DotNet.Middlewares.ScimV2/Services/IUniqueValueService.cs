using Looplex.DotNet.Middlewares.ScimV2.Entities;

namespace Looplex.DotNet.Middlewares.ScimV2.Services;

public interface IUniqueValueService<TRes> where TRes : Resource
{
    bool IsUnique<TVal>(string propertyName, TVal value) where TVal : struct;
}