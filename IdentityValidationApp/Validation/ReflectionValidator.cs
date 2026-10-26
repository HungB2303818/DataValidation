using System;
using System.Reflection;
using System.Text.RegularExpressions;
using IdentityValidationApp.Attributes;
using IdentityValidationApp.Models;

namespace IdentityValidationApp.Validation
{
    public static class ReflectionValidator
    {
        public static ValidationResult Validate<T>(T model) where T : class
        {
            var result = new ValidationResult();
            if (model == null)
            {
                result.Errors["Model"] = "Dữ liệu đầu vào rỗng (null).";
                return result;
            }

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in properties)
            {
                var regexAttr = prop.GetCustomAttribute<RegexValidationAttribute>(inherit: true);
                if (regexAttr == null) continue;

                string rawValue = prop.GetValue(model)?.ToString()?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(rawValue) ||
                    !Regex.IsMatch(rawValue, regexAttr.Pattern, RegexOptions.Compiled))
                {
                    result.Errors[prop.Name] = $"{regexAttr.FieldDisplayName}: {regexAttr.ErrorMessage}";
                }
            }

            return result;
        }
    }
}