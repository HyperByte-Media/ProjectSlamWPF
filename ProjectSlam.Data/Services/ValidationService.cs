using ProjectSlam.Data.Interfaces;
using ProjectSlam.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace ProjectSlam.Data.Services;

public class ValidationService : IValidationService
{
    private readonly Dictionary<string, ValidationRule> _validationRules;

    public ValidationService()
    {
        _validationRules = new Dictionary<string, ValidationRule>
        {
            // Basic Information
            ["FirstName"] = new ValidationRule
            {
                Required = true,
                MinLength = 1,
                MaxLength = 50,
                ErrorMessage = "First Name is required and must be between 1 and 50 characters"
            },
            ["LastName"] = new ValidationRule
            {
                Required = true,
                MinLength = 1,
                MaxLength = 50,
                ErrorMessage = "Last Name is required and must be between 1 and 50 characters"
            },
            ["Sex"] = new ValidationRule
            {
                Required = true,
                ErrorMessage = "Sex is required"
            },
            ["Height"] = new ValidationRule
            {
                Required = true,
                Pattern = @"^\d'(?:\d|1[0-1])""$",
                ErrorMessage = "Height must be in format: F'II\" (e.g., 5'10\")"
            },
            ["Weight"] = new ValidationRule
            {
                Required = true,
                Pattern = @"^\d{2,3}$",
                Validate = (value) =>
                {
                    if (int.TryParse(value?.ToString(), out int weight))
                    {
                        if (weight < 50 || weight > 999)
                            return "Weight must be between 50 and 999 pounds";
                    }
                    return null;
                },
                ErrorMessage = "Weight must be between 50 and 999 pounds"
            }
        };
    }

    public bool ValidateEntity(object entity, out ICollection<ValidationResult> results)
    {
        results = new List<ValidationResult>();
        if (entity == null)
            return false;

        var context = new ValidationContext(entity);
        return Validator.TryValidateObject(entity, context, results, validateAllProperties: true);
    }

    public bool ValidateProperty(object value, ValidationContext context, out ICollection<ValidationResult> results)
    {
        results = new List<ValidationResult>();

        if (!_validationRules.TryGetValue(context.MemberName, out var rule))
            return true;

        if (rule.Required && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
        {
            results.Add(new ValidationResult(rule.ErrorMessage, new[] { context.MemberName }));
            return false;
        }

        var stringValue = value?.ToString() ?? string.Empty;

        // Custom validation if provided
        if (rule.Validate != null)
        {
            var customError = rule.Validate(value);
            if (customError != null)
            {
                results.Add(new ValidationResult(customError, new[] { context.MemberName }));
                return false;
            }
        }

        if (rule.MinLength.HasValue && stringValue.Length < rule.MinLength)
        {
            results.Add(new ValidationResult(rule.ErrorMessage, new[] { context.MemberName }));
            return false;
        }

        if (rule.MaxLength.HasValue && stringValue.Length > rule.MaxLength)
        {
            results.Add(new ValidationResult(rule.ErrorMessage, new[] { context.MemberName }));
            return false;
        }

        if (!string.IsNullOrEmpty(rule.Pattern) && !Regex.IsMatch(stringValue, rule.Pattern))
        {
            results.Add(new ValidationResult(rule.ErrorMessage, new[] { context.MemberName }));
            return false;
        }

        return true;
    }
}