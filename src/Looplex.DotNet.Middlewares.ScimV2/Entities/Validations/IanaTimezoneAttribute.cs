using System.ComponentModel.DataAnnotations;
using Microsoft.Net.Http.Headers;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

public class IanaTimezoneAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var valid = true;
        var strValue = (string?)value;
        if (string.IsNullOrEmpty(strValue)) return valid;
        
        valid = TimeZoneInfo.TryFindSystemTimeZoneById(strValue, out var _);
        
        return valid;
    }
}