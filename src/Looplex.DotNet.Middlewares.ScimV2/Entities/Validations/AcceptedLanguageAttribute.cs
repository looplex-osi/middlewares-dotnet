using System.ComponentModel.DataAnnotations;
using Microsoft.Net.Http.Headers;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

public class AcceptedLanguageAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var valid = true;
        var strValue = (string?)value;
        if (string.IsNullOrEmpty(strValue)) return valid;
        
        if (!StringWithQualityHeaderValue.TryParseList(strValue.Split(','), out var list) 
            || !list.All(str => LanguageTagAttribute.IsValidCultureName(str.Value.ToString())))
        {
            valid = false;
        }

        return valid;
    }
}