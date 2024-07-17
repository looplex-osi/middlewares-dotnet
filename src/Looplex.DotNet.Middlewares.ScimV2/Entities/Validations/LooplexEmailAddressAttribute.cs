using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

/// <summary>Validates an email address.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class LooplexEmailAddressAttribute : ValidationAttribute
{
    public ErrorType ErrorType { get; set; } = ErrorType.EmailAddressInvalid;

    private readonly EmailAddressAttribute _emailAddressAttribute = new EmailAddressAttribute();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage = string.Format(ErrorType.GetDescription(), validationContext.MemberName);

        return _emailAddressAttribute.IsValid(value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }

    public override bool RequiresValidationContext => _emailAddressAttribute.RequiresValidationContext;

    public override string FormatErrorMessage(string name)
    {
        return _emailAddressAttribute.FormatErrorMessage(name);
    }
}