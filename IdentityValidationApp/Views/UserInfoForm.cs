using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using IdentityValidationApp.Models;

namespace IdentityValidationApp.Views
{
    public class UserInfoForm : Form, IUserInfoView
    {
        private static readonly Color Primary = Color.FromArgb(41, 128, 185);
        private static readonly Color Surface = Color.FromArgb(245, 247, 250);
        private static readonly Color OkFore = Color.FromArgb(39, 174, 96);
        private static readonly Color OkBack = Color.FromArgb(234, 247, 238);
        private static readonly Color ErrFore = Color.FromArgb(192, 57, 43);
        private static readonly Color ErrBack = Color.FromArgb(253, 237, 236);

        // Controls nhập liệu
        private TextBox _txtFullName = null!;
        private TextBox _txtPersonalId = null!;
        private TextBox _txtEmail = null!;
        private TextBox _txtPhone = null!;
        private TextBox _txtStartDate = null!;

        private Button _btnValidate = null!;
        private Label _lblHeaderTitle = null!;

        private Panel _pnlResult = null!;
        private Label _lblResultTitle = null!;
        private RichTextBox _rtbErrorDetails = null!;

        // Thực thi các Properties của IUserInfoView
        public int CitizenId { get; set; } = 0;
        public string FullName { get => _txtFullName.Text.Trim(); set => _txtFullName.Text = value; }
        public string PersonalId { get => _txtPersonalId.Text.Trim(); set => _txtPersonalId.Text = value; }
        public string Email { get => _txtEmail.Text.Trim(); set => _txtEmail.Text = value; }
        public string PhoneNumber { get => _txtPhone.Text.Trim(); set => _txtPhone.Text = value; }
        public string StartDate { get => _txtStartDate.Text.Trim(); set => _txtStartDate.Text = value; }

        public event EventHandler? ValidateRequested;

        public UserInfoForm(CitizenModel? modelToEdit = null)
        {
            InitializeComponent();

            // Nếu truyền vào Model => Chuyển sang chế độ Cập nhật (Edit Mode)
            if (modelToEdit != null)
            {
                CitizenId = modelToEdit.Id;
                FullName = modelToEdit.FullName;
                PersonalId = modelToEdit.PersonalId;
                Email = modelToEdit.Email;
                PhoneNumber = modelToEdit.PhoneNumber;
                StartDate = modelToEdit.StartDate;

                Text = "Cập nhật thông tin nhân viên";
                _lblHeaderTitle.Text = "Cập nhật thông tin nhân viên";
                _btnValidate.Text = "Cập nhật thông tin";
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Text = "Thêm mới nhân viên";
            ClientSize = new Size(520, 690);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Surface;
            Font = new Font("Segoe UI", 10);

            // 1. Header Panel
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 78, BackColor = Primary };
            _lblHeaderTitle = new Label { Text = "Thêm mới thông tin nhân viên", ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 16), AutoSize = true, Location = new Point(24, 12) };
            pnlHeader.Controls.Add(_lblHeaderTitle);
            pnlHeader.Controls.Add(new Label { Text = "Xác thực bằng Regex, Attribute Reflection & Lưu MySQL", ForeColor = Color.FromArgb(215, 235, 250), Font = new Font("Segoe UI", 9), AutoSize = true, Location = new Point(26, 46) });
            Controls.Add(pnlHeader);

            // 2. Họ và tên
            Controls.Add(new Label { Text = "Họ và tên nhân viên:", AutoSize = true, Location = new Point(24, 90), Font = new Font("Segoe UI Semibold", 10) });
            _txtFullName = new TextBox { Location = new Point(24, 115), Size = new Size(472, 30), Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(_txtFullName);

            // 3. Số định danh CCCD
            Controls.Add(new Label { Text = "Số định danh (CCCD 12 số):", AutoSize = true, Location = new Point(24, 155), Font = new Font("Segoe UI Semibold", 10) });
            _txtPersonalId = new TextBox { Location = new Point(24, 180), Size = new Size(472, 30), Font = new Font("Consolas", 11), BorderStyle = BorderStyle.FixedSingle, MaxLength = 12 };
            Controls.Add(_txtPersonalId);

            // 4. Email
            Controls.Add(new Label { Text = "Địa chỉ Email:", AutoSize = true, Location = new Point(24, 220), Font = new Font("Segoe UI Semibold", 10) });
            _txtEmail = new TextBox { Location = new Point(24, 245), Size = new Size(472, 30), Font = new Font("Consolas", 11), BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(_txtEmail);

            // 5. Số điện thoại
            Controls.Add(new Label { Text = "Số điện thoại:", AutoSize = true, Location = new Point(24, 285), Font = new Font("Segoe UI Semibold", 10) });
            _txtPhone = new TextBox { Location = new Point(24, 310), Size = new Size(472, 30), Font = new Font("Consolas", 11), BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(_txtPhone);

            // 6. Ngày bắt đầu làm việc
            Controls.Add(new Label { Text = "Ngày bắt đầu làm việc (YYYY-MM-DD):", AutoSize = true, Location = new Point(24, 350), Font = new Font("Segoe UI Semibold", 10) });
            _txtStartDate = new TextBox { Location = new Point(24, 375), Size = new Size(472, 30), Font = new Font("Consolas", 11), BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(_txtStartDate);

            // 7. Buttons
            _btnValidate = new Button { Text = "Xác thực & Lưu dữ liệu", Location = new Point(24, 425), Size = new Size(340, 40), Font = new Font("Segoe UI Semibold", 10), BackColor = Primary, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            _btnValidate.FlatAppearance.BorderSize = 0;
            _btnValidate.Click += (s, e) => ValidateRequested?.Invoke(this, EventArgs.Empty);
            Controls.Add(_btnValidate);

            var btnClear = new Button { Text = "Xóa form", Location = new Point(376, 425), Size = new Size(120, 40), Font = new Font("Segoe UI", 10), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnClear.Click += (s, e) => ClearForm();
            Controls.Add(btnClear);

            AcceptButton = _btnValidate;

            // 8. Result Panel
            _pnlResult = new Panel { Location = new Point(24, 485), Size = new Size(472, 180), BackColor = Color.White, Visible = false };
            _lblResultTitle = new Label { Location = new Point(16, 12), Size = new Size(440, 26), Font = new Font("Segoe UI Semibold", 12) };
            _rtbErrorDetails = new RichTextBox { Location = new Point(16, 40), Size = new Size(440, 130), ReadOnly = true, BorderStyle = BorderStyle.None, BackColor = Color.White, Font = new Font("Segoe UI", 9.5f) };

            _pnlResult.Controls.Add(_lblResultTitle);
            _pnlResult.Controls.Add(_rtbErrorDetails);
            Controls.Add(_pnlResult);

            // Bắt sự kiện thay đổi dữ liệu để ẩn thông báo lỗi cũ
            EventHandler resetResult = (s, e) => _pnlResult.Visible = false;
            _txtFullName.TextChanged += resetResult;
            _txtPersonalId.TextChanged += resetResult;
            _txtEmail.TextChanged += resetResult;
            _txtPhone.TextChanged += resetResult;
            _txtStartDate.TextChanged += resetResult;

            ResumeLayout(false);
            PerformLayout();
        }

        private void ClearForm()
        {
            _txtFullName.Clear();
            _txtPersonalId.Clear();
            _txtEmail.Clear();
            _txtPhone.Clear();
            _txtStartDate.Clear();
            _pnlResult.Visible = false;
            _txtFullName.Focus();
        }

        public void ShowResult(bool isValid, Dictionary<string, string> errors, string customMessage = "")
        {
            _pnlResult.Visible = true;
            _rtbErrorDetails.Clear();

            if (isValid)
            {
                _pnlResult.BackColor = OkBack;
                _lblResultTitle.ForeColor = OkFore;
                _lblResultTitle.Text = "✓ THÔNG TIN HỢP LỆ";
                _rtbErrorDetails.BackColor = OkBack;
                _rtbErrorDetails.ForeColor = OkFore;
                _rtbErrorDetails.Text = string.IsNullOrEmpty(customMessage)
                    ? "Tất cả dữ liệu nhập vào đều đúng định dạng chuẩn."
                    : customMessage;
            }
            else
            {
                _pnlResult.BackColor = ErrBack;
                _lblResultTitle.ForeColor = ErrFore;
                _lblResultTitle.Text = $"✗ PHÁT HIỆN {errors.Count} LỖI DỮ LIỆU";
                _rtbErrorDetails.BackColor = ErrBack;
                _rtbErrorDetails.ForeColor = ErrFore;

                string errorMessages = string.Join("\n", errors.Select(e => $"• {e.Value}"));
                if (!string.IsNullOrEmpty(customMessage))
                {
                    errorMessages += $"\n• {customMessage}";
                }
                _rtbErrorDetails.Text = errorMessages;
            }
        }

        public void CloseView()
        {
            Close();
        }
    }
}