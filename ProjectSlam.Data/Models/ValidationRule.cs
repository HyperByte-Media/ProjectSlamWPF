namespace ProjectSlam.Data.Models;

public class ValidationRule
{
    public bool Required { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public string Pattern { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public Func<object, string> Validate { get; set; }
}
