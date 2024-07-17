using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class LooplexUrlAttribute : ValidationAttribute
{
    public ErrorType ErrorType { get; set; } = ErrorType.UrlInvalid;

    private readonly UrlAttribute _urlAttribute = new();

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage = string.Format(ErrorType.GetDescription(), validationContext.MemberName);

        return _urlAttribute.IsValid(value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }

    public override bool RequiresValidationContext => _urlAttribute.RequiresValidationContext;

    public override string FormatErrorMessage(string name)
    {
        return _urlAttribute.FormatErrorMessage(name);
    }
}