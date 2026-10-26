using System.Collections.Generic;

namespace IdentityValidationApp.Models
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();
    }
}