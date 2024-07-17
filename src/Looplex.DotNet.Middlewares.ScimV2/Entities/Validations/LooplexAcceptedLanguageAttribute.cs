using System.ComponentModel.DataAnnotations;
using Microsoft.Net.Http.Headers;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class LooplexAcceptedLanguageAttribute : ValidationAttribute
{
    public ErrorType ErrorType { get; set; } = ErrorType.PropertyInvalid;

    public override bool IsValid(object? value)
    {
        var valid = true;
        var strValue = (string?)value;
        if (string.IsNullOrEmpty(strValue)) return valid;

        if (!StringWithQualityHeaderValue.TryParseList(strValue.Split(','), out var list)
            || !list.All(str => LooplexLanguageTagAttribute.IsValidCultureName(str.Value.ToString())))
            valid = false;

        return valid;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage = string.Format(ErrorType.GetDescription(), validationContext.MemberName);

        return base.IsValid(value, validationContext);
    }
}