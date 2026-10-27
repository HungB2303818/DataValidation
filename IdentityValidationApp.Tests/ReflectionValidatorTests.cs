using Xunit;
using IdentityValidationApp.Models;
using IdentityValidationApp.Validation;

namespace IdentityValidationApp.Tests
{
    public class ReflectionValidatorTests
    {
        [Fact]
        public void Validate_ValidCitizen_ShouldReturnNoErrors()
        {
            // Arrange: Dữ liệu hợp lệ 100%
            var citizen = new CitizenModel
            {
                FullName = "Nguyễn Văn Anh",
                PersonalId = "012345678901",
                Email = "vananh.nguyen@gmail.com",
                PhoneNumber = "0987654321",
                StartDate = "2026-10-27"
            };

            // Act
            ValidationResult result = ReflectionValidator.Validate(citizen);

            // Assert: Danh sách lỗi phải rỗng
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_NullModel_ShouldReturnModelError()
        {
            // Arrange
            CitizenModel? citizen = null;

            // Act
            ValidationResult result = ReflectionValidator.Validate(citizen!);

            // Assert: Phải chứa khóa "Model" trong Errors
            Assert.True(result.Errors.ContainsKey("Model"));
            Assert.Equal("Dữ liệu đầu vào rỗng (null).", result.Errors["Model"]);
        }

        [Theory]
        [InlineData("0123456789")]       // Thiếu số (10 số)
        [InlineData("0123456789012")]    // Thừa số (13 số)
        [InlineData("01234567890a")]     // Chứa chữ cái
        [InlineData("")]                 // Trống
        public void Validate_InvalidPersonalId_ShouldReturnPersonalIdError(string invalidPersonalId)
        {
            // Arrange
            var citizen = new CitizenModel
            {
                FullName = "Nguyễn Văn A",
                PersonalId = invalidPersonalId,
                Email = "test@gmail.com",
                PhoneNumber = "0987654321",
                StartDate = "2026-10-27"
            };

            // Act
            ValidationResult result = ReflectionValidator.Validate(citizen);

            // Assert: Phải báo lỗi đúng thuộc tính PersonalId
            Assert.True(result.Errors.ContainsKey("PersonalId"));
        }

        [Theory]
        [InlineData("0123456789")]       // Đầu số không thuộc nhà mạng VN
        [InlineData("1234567890")]       // Không bắt đầu bằng 0 hoặc +84
        [InlineData("098765432")]        // Thiếu số (9 số)
        public void Validate_InvalidPhoneNumber_ShouldReturnPhoneNumberError(string invalidPhone)
        {
            // Arrange
            var citizen = new CitizenModel
            {
                FullName = "Nguyễn Văn A",
                PersonalId = "012345678901",
                Email = "test@gmail.com",
                PhoneNumber = invalidPhone,
                StartDate = "2026-10-27"
            };

            // Act
            ValidationResult result = ReflectionValidator.Validate(citizen);

            // Assert: Phải báo lỗi đúng thuộc tính PhoneNumber
            Assert.True(result.Errors.ContainsKey("PhoneNumber"));
        }

        [Theory]
        [InlineData("user@domain")]        // Thiếu tên miền cấp cao (.com, .vn)
        [InlineData("@gmail.com")]         // Thiếu username
        [InlineData("user name@gmail.com")]// Chứa khoảng trắng
        public void Validate_InvalidEmail_ShouldReturnEmailError(string invalidEmail)
        {
            // Arrange
            var citizen = new CitizenModel
            {
                FullName = "Nguyễn Văn A",
                PersonalId = "012345678901",
                Email = invalidEmail,
                PhoneNumber = "0987654321",
                StartDate = "2026-10-27"
            };

            // Act
            ValidationResult result = ReflectionValidator.Validate(citizen);

            // Assert: Phải báo lỗi đúng thuộc tính Email
            Assert.True(result.Errors.ContainsKey("Email"));
        }

        [Theory]
        [InlineData("27/10/2026")]       // Định dạng DD/MM/YYYY thay vì YYYY-MM-DD
        [InlineData("2026-13-01")]       // Tháng 13 không tồn tại
        [InlineData("2026-10-32")]       // Ngày 32 không tồn tại
        public void Validate_InvalidStartDate_ShouldReturnStartDateError(string invalidDate)
        {
            // Arrange
            var citizen = new CitizenModel
            {
                FullName = "Nguyễn Văn A",
                PersonalId = "012345678901",
                Email = "test@gmail.com",
                PhoneNumber = "0987654321",
                StartDate = invalidDate
            };

            // Act
            ValidationResult result = ReflectionValidator.Validate(citizen);

            // Assert: Phải báo lỗi đúng thuộc tính StartDate
            Assert.True(result.Errors.ContainsKey("StartDate"));
        }
    }
}