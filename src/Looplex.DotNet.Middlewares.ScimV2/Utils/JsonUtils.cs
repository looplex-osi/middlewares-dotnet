using Newtonsoft.Json.Linq;

namespace Looplex.DotNet.Middlewares.ScimV2.Utils;

/// <summary>
/// TODO move to core package
/// </summary>
public static class JsonUtils
{
    public static void Traverse(JToken token, Action<JToken> visitor)
    {
        visitor(token);

        foreach (var child in token.Children())
        {
            Traverse(child, visitor);
        }
    }
}