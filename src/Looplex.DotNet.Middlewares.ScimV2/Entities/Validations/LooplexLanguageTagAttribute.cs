using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class LooplexLanguageTagAttribute : ValidationAttribute
{
    public ErrorType ErrorType { get; set; } = ErrorType.PropertyInvalid;

    public override bool IsValid(object? value)
    {
        var valid = true;
        var strValue = (string?)value;
        if (string.IsNullOrEmpty(strValue)) return valid;

        valid = strValue.Split(',', StringSplitOptions.RemoveEmptyEntries).All(c => IsValidCultureName(c.Trim()));

        return valid;
    }

    internal static bool IsValidCultureName(string cultureName)
    {
        var valid = true;
        try
        {
            _ = new CultureInfo(cultureName);
            return CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Any(c => c.Name.Equals(cultureName, StringComparison.OrdinalIgnoreCase));
        }
        catch (CultureNotFoundException)
        {
            valid = false;
        }

        return valid;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage = string.Format(ErrorType.GetDescription(), validationContext.MemberName);

        return base.IsValid(value, validationContext);
    }
}