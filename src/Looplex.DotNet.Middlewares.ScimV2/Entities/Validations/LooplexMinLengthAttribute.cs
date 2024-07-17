using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class LooplexMinLengthAttribute(int length) : MinLengthAttribute(length)
{
    public ErrorType ErrorType { get; set; } = ErrorType.StringRequiredMinLengh;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage = string.Format(ErrorType.GetDescription(), validationContext.MemberName);

        return base.IsValid(value, validationContext);
    }
}