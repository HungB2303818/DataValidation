using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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

        private TextBox _txtPersonalId = null!;
        private TextBox _txtEmail = null!;
        private TextBox _txtPhone = null!;
        private Button _btnValidate = null!;

        private Panel _pnlResult = null!;
        private Label _lblResultTitle = null!;
        private RichTextBox _rtbErrorDetails = null!;

        public string PersonalId => _txtPersonalId.Text.Trim();
        public string Email => _txtEmail.Text.Trim();
        public string PhoneNumber => _txtPhone.Text.Trim();

        public event EventHandler? ValidateRequested;

        public UserInfoForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Text = "Hệ thống kiểm tra thông tin công dân";
            ClientSize = new Size(520, 560);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Surface;
            Font = new Font("Segoe UI", 10);

            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 78, BackColor = Primary };
            pnlHeader.Controls.Add(new Label { Text = "Kiểm tra thông tin công dân", ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 16), AutoSize = true, Location = new Point(24, 12) });
            pnlHeader.Controls.Add(new Label { Text = "Xác thực bằng Regex, Attribute Reflection & Lưu MySQL", ForeColor = Color.FromArgb(215, 235, 250), Font = new Font("Segoe UI", 9), AutoSize = true, Location = new Point(26, 46) });
            Controls.Add(pnlHeader);

            Controls.Add(new Label { Text = "Số định danh (CCCD 12 số):", AutoSize = true, Location = new Point(24, 96), Font = new Font("Segoe UI Semibold", 10) });
            _txtPersonalId = new TextBox { Location = new Point(24, 122), Size = new Size(472, 30), Font = new Font("Consolas", 12), BorderStyle = BorderStyle.FixedSingle, MaxLength = 12 };
            Controls.Add(_txtPersonalId);

            Controls.Add(new Label { Text = "Địa chỉ Email:", AutoSize = true, Location = new Point(24, 166), Font = new Font("Segoe UI Semibold", 10) });
            _txtEmail = new TextBox { Location = new Point(24, 192), Size = new Size(472, 30), Font = new Font("Consolas", 12), BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(_txtEmail);

            Controls.Add(new Label { Text = "Số điện thoại:", AutoSize = true, Location = new Point(24, 236), Font = new Font("Segoe UI Semibold", 10) });
            _txtPhone = new TextBox { Location = new Point(24, 262), Size = new Size(472, 30), Font = new Font("Consolas", 12), BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(_txtPhone);

            _btnValidate = new Button { Text = "Xác thực & Lưu dữ liệu", Location = new Point(24, 316), Size = new Size(340, 40), Font = new Font("Segoe UI Semibold", 10), BackColor = Primary, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            _btnValidate.FlatAppearance.BorderSize = 0;
            _btnValidate.Click += (s, e) => ValidateRequested?.Invoke(this, EventArgs.Empty);
            Controls.Add(_btnValidate);

            var btnClear = new Button { Text = "Xóa form", Location = new Point(376, 316), Size = new Size(120, 40), Font = new Font("Segoe UI", 10), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnClear.Click += (s, e) => { _txtPersonalId.Clear(); _txtEmail.Clear(); _txtPhone.Clear(); _pnlResult.Visible = false; _txtPersonalId.Focus(); };
            Controls.Add(btnClear);

            AcceptButton = _btnValidate;

            _pnlResult = new Panel { Location = new Point(24, 380), Size = new Size(472, 140), BackColor = Color.White, Visible = false };
            _lblResultTitle = new Label { Location = new Point(16, 12), Size = new Size(440, 26), Font = new Font("Segoe UI Semibold", 12) };
            _rtbErrorDetails = new RichTextBox { Location = new Point(16, 40), Size = new Size(440, 90), ReadOnly = true, BorderStyle = BorderStyle.None, BackColor = Color.White, Font = new Font("Segoe UI", 9.5f) };

            _pnlResult.Controls.Add(_lblResultTitle);
            _pnlResult.Controls.Add(_rtbErrorDetails);
            Controls.Add(_pnlResult);

            EventHandler resetResult = (s, e) => _pnlResult.Visible = false;
            _txtPersonalId.TextChanged += resetResult;
            _txtEmail.TextChanged += resetResult;
            _txtPhone.TextChanged += resetResult;

            ResumeLayout(false);
            PerformLayout();
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
    }
}