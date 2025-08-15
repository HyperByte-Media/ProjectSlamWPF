using System.ComponentModel.DataAnnotations;

namespace ProjectSlam.Data.Interfaces;

public interface IValidationService
{
    bool ValidateEntity(object entity, out ICollection<ValidationResult> results);
    bool ValidateProperty(object value, ValidationContext context, out ICollection<ValidationResult> results);
}
