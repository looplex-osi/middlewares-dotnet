using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

public class NullOrNotEmptyAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var strValue = (string?)value;
        return strValue == null || !string.IsNullOrWhiteSpace(strValue);
    }
}