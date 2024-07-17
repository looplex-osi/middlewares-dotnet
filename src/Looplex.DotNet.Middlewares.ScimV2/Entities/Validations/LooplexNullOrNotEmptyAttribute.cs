using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class LooplexNullOrNotEmptyAttribute : ValidationAttribute
{
    public ErrorType ErrorType { get; set; } = ErrorType.StringEmpty;

    public override bool IsValid(object? value)
    {
        var strValue = (string?)value;
        return strValue == null || !string.IsNullOrWhiteSpace(strValue);
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage = string.Format(ErrorType.GetDescription(), validationContext.MemberName);

        return base.IsValid(value, validationContext);
    }
}