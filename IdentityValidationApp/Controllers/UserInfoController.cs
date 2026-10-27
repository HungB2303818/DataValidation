using System;
using System.Windows.Forms;
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
            // Trích xuất dữ liệu từ View bao gồm cả FullName, StartDate và CitizenId
            var model = new CitizenModel
            {
                Id = _view.CitizenId,
                FullName = _view.FullName,
                PersonalId = _view.PersonalId,
                Email = _view.Email,
                PhoneNumber = _view.PhoneNumber,
                StartDate = _view.StartDate
            };

            // 1. Kiểm tra định dạng phía ứng dụng bằng Custom Attributes & Reflection Validator
            ValidationResult result = ReflectionValidator.Validate(model);
            if (!result.IsValid)
            {
                _view.ShowResult(false, result.Errors);
                return;
            }

            // 2. Lưu mới hoặc Cập nhật vào MySQL
            try
            {
                bool isSuccess = model.Id == 0
                    ? await _repository.InsertCitizenAsync(model)
                    : await _repository.UpdateCitizenAsync(model);

                if (isSuccess)
                {
                    string successMsg = model.Id == 0
                        ? "Thêm mới thông tin nhân viên thành công!"
                        : "Cập nhật thông tin nhân viên thành công!";

                    MessageBox.Show(successMsg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Đóng dialog sau khi hoàn tất
                    _view.CloseView();
                }
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                result.Errors["Database"] = "Số định danh (CCCD) hoặc Email đã tồn tại trong hệ thống.";
                _view.ShowResult(false, result.Errors);
            }
            catch (Exception ex)
            {
                result.Errors["Database"] = $"Lỗi kết nối / xử lý MySQL: {ex.Message}";
                _view.ShowResult(false, result.Errors);
            }
        }
    }
}