using System.Net;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Messages;
using Looplex.DotNet.Middlewares.ScimV2.Domain.ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Looplex.DotNet.Middlewares.ScimV2.Middlewares;

public static partial class ScimV2Middlewares
{
    public static readonly MiddlewareDelegate AttributesMiddleware = async (context, cancellationToken, next) =>
    {
        await next();

        cancellationToken.ThrowIfCancellationRequested();
        
        JObject? json = null;
        if (context.Result != null)
        {
            if (context.Result is string str)
                json = JObject.Parse(str);
            else if (context.Result is JObject token)
                json = token;
        }

        if (json == null)
            return;

        if (context.AsScimV2Context().Query.ContainsKey("attributes"))
            throw new Error("Attributes query parameter not supported", (int)HttpStatusCode.NotImplemented);
        var excludedAttributes = (context.GetQuery("excludedAttributes") ?? "")
            .Split(",", StringSplitOptions.RemoveEmptyEntries);
        
        try
        {
            foreach (var attribute in excludedAttributes)
            {
                var tokens = json.SelectTokens(attribute);
                foreach (var token in tokens)
                {
                    if (token is JValue)
                        token.Parent?.Remove();    
                    else 
                        token.Remove();
                }
            }
            
            if (context.Result is string)
                context.Result = JsonConvert.SerializeObject(json);
        }
        catch (Exception)
        {
            // ignored
        }
    };
}