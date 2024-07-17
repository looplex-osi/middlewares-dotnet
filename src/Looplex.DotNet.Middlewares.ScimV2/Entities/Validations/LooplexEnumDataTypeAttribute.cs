using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field |
                AttributeTargets.Parameter)]
public sealed class LooplexEnumDataTypeAttribute(Type enumType) : ValidationAttribute
{
    public ErrorType ErrorType { get; set; } = ErrorType.OptionInvalid;

    private readonly EnumDataTypeAttribute _enumDataTypeAttribute = new(enumType);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage = string.Format(ErrorType.GetDescription(), validationContext.MemberName);

        return _enumDataTypeAttribute.IsValid(value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
    }

    public Type EnumType => _enumDataTypeAttribute.EnumType;

    public override bool RequiresValidationContext => _enumDataTypeAttribute.RequiresValidationContext;

    public override string FormatErrorMessage(string name)
    {
        return _enumDataTypeAttribute.FormatErrorMessage(name);
    }
}