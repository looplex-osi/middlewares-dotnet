using System.ComponentModel.DataAnnotations;

namespace Looplex.DotNet.Middlewares.ScimV2.Entities.Validations;

public class ValidAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        if (value == null) return ValidationResult.Success;
        
        var context = new ValidationContext(value, null, null);
        bool isValid;
        if (value is IEnumerable<object> enumerable)
        {
            isValid = enumerable.All(o => Validator.TryValidateObject(value, context, results, true));
        }
        else
        {
            isValid = Validator.TryValidateObject(value, context, results, true);
        }

        if (isValid)
        {
            return ValidationResult.Success;
        }

        var compositeResults = new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed!");
        results.ForEach(compositeResults.AddResult);

        return compositeResults;
    }
}

public class CompositeValidationResult : ValidationResult
{
    private readonly List<ValidationResult> _results = [];

    public IEnumerable<ValidationResult> Results => _results;

    public CompositeValidationResult(string errorMessage) : base(errorMessage) { }

    public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { }

    protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

    public void AddResult(ValidationResult validationResult)
    {
        _results.Add(validationResult);
    }
}