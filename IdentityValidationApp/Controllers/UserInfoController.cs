using System;
using MySqlConnector;
using IdentityValidationApp.Views;
using IdentityValidationApp.Models;
using IdentityValidationApp.Validation;
using IdentityValidationApp.Repositories;

namespace IdentityValidationApp.Controllers
{
    public class UserInfoController
    {
        private readonly IUserInfoView _view;
        private readonly CitizenRepository _repository;

        public UserInfoController(IUserInfoView view, CitizenRepository repository)
        {
            _view = view;
            _repository = repository;
            _view.ValidateRequested += OnValidateRequested;
        }

        private async void OnValidateRequested(object? sender, EventArgs e)
        {
            var model = new CitizenModel
            {
                PersonalId = _view.PersonalId,
                Email = _view.Email,
                PhoneNumber = _view.PhoneNumber
            };

            // 1. Kiểm tra định dạng phía ứng dụng bằng Reflection
            ValidationResult result = ReflectionValidator.Validate(model);
            if (!result.IsValid)
            {
                _view.ShowResult(false, result.Errors);
                return;
            }

            // 2. Lưu vào MySQL bằng Parameterized Query
            try
            {
                bool isInserted = await _repository.InsertCitizenAsync(model);
                if (isInserted)
                {
                    _view.ShowResult(true, result.Errors, "Xác thực hợp lệ & Đã lưu vào MySQL thành công!");
                }
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                result.Errors["Database"] = "Số định danh (CCCD) hoặc Email đã tồn tại trong hệ thống.";
                _view.ShowResult(false, result.Errors);
            }
            catch (Exception ex)
            {
                result.Errors["Database"] = $"Lỗi kết nối MySQL: {ex.Message}";
                _view.ShowResult(false, result.Errors);
            }
        }
    }
}