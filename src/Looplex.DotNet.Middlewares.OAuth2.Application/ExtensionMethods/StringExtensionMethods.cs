namespace Looplex.DotNet.Middlewares.OAuth2.Application.ExtensionMethods;

public static class StringExtensionMethods
{
    private const string Basic = "Basic ";
    private const string Bearer = "Bearer ";
    
    public static bool IsBasicAuthentication(this string value, out string? token)
    {
        token = null;
        var result = false;
        
        if (value.StartsWith(Basic, StringComparison.OrdinalIgnoreCase))
        {
            token = value[Basic.Length..];
            result = true;
        }

        return result;
    }
    
    public static bool IsBearerAuthentication(this string value, out string? token)
    {
        token = null;
        var result = false;
        
        if (value.StartsWith(Bearer, StringComparison.OrdinalIgnoreCase))
        {
            token = value[Bearer.Length..];
            result = true;
        }

        return result;
    }
}