using System;

namespace IdentityValidationApp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RegexValidationAttribute : Attribute
    {
        public string FieldDisplayName { get; }
        public string Pattern { get; }
        public string ErrorMessage { get; }

        public RegexValidationAttribute(string fieldDisplayName, string pattern, string errorMessage)
        {
            FieldDisplayName = fieldDisplayName;
            Pattern = pattern;
            ErrorMessage = errorMessage;
        }
    }

    public class PersonalIdAttribute : RegexValidationAttribute
    {
        public PersonalIdAttribute() :   base(
            "Số định danh (CCCD)",
            @"^\d{12}$",
            "Số định danh phải bao gồm đúng 12 chữ số.")
        { }
    }

    public class FullNameAttribute : RegexValidationAttribute
    {
        public FullNameAttribute() : base(
            "Họ và tên",
            @"^[a-zA-ZÀ-ỹ\s]{2,100}$",
            "Họ và tên chỉ được chứa chữ cái, khoảng trắng và dài từ 2-100 ký tự.")
        { }
    }

    public class CustomEmailAttribute : RegexValidationAttribute
    {
        public CustomEmailAttribute() : base(
            "Email",
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            "Địa chỉ email không đúng định dạng chuẩn (vd: ten@domain.com).")
        { }
    }

    public class VietnamPhoneAttribute : RegexValidationAttribute
    {
        public VietnamPhoneAttribute() : base(
            "Số điện thoại",
            @"^(0|\+84)(3|5|7|8|9)\d{8}$",
            "Số điện thoại phải bắt đầu bằng 0 hoặc +84 và đủ 10 chữ số.")
        { }
    }

    public class StartDateAttribute : RegexValidationAttribute
    {
        public StartDateAttribute() : base(
            "Ngày bắt đầu làm",
            @"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])$",
            "Ngày bắt đầu làm việc phải đúng định dạng YYYY-MM-DD.")
        { }
    }
}