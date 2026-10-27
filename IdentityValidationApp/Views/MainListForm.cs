using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using IdentityValidationApp.Models;
using IdentityValidationApp.Repositories;
using IdentityValidationApp.Controllers;

namespace IdentityValidationApp.Views
{
    public class MainListForm : Form
    {
        private static readonly Color Primary = Color.FromArgb(41, 128, 185);
        private static readonly Color Surface = Color.FromArgb(245, 247, 250);

        private DataGridView _grid = null!;
        private Button _btnAdd = null!;
        private readonly CitizenRepository _repository;

        public MainListForm(CitizenRepository repository)
        {
            _repository = repository;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Text = "Hệ thống Quản lý Nhân viên";
            Size = new Size(960, 560);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Surface;
            Font = new Font("Segoe UI", 10);

            // Header Panel
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Primary };
            pnlHeader.Controls.Add(new Label
            {
                Text = "DANH SÁCH NHÂN VIÊN",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 16),
                AutoSize = true,
                Location = new Point(20, 18)
            });
            Controls.Add(pnlHeader);

            // Nút Thêm Mới
            _btnAdd = new Button
            {
                Text = "+ Thêm mới nhân viên",
                Location = new Point(20, 85),
                Size = new Size(180, 40),
                BackColor = Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10),
                Cursor = Cursors.Hand
            };
            _btnAdd.FlatAppearance.BorderSize = 0;
            _btnAdd.Click += BtnAdd_Click;
            Controls.Add(_btnAdd);

            // DataGridView Danh sách Nhân viên
            _grid = new DataGridView
            {
                Location = new Point(20, 135),
                Size = new Size(904, 360),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                RowHeadersVisible = false,
                RowTemplate = { Height = 38 }
            };

            // Cấu hình các cột cố định
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FullName", HeaderText = "Họ và tên", Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PersonalId", HeaderText = "CCCD", Width = 130 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PhoneNumber", HeaderText = "SĐT", Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "StartDate", HeaderText = "Ngày làm việc", Width = 110 });

            // Cột Thao tác duy nhất chiếm toàn bộ khoảng trống còn lại (Fill)
            var actionCol = new DataGridViewTextBoxColumn
            {
                Name = "colActions",
                HeaderText = "Thao tác",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 140,
                ReadOnly = true
            };
            _grid.Columns.Add(actionCol);

            // Đăng ký sự kiện vẽ giao diện nút và bắt click
            _grid.CellPainting += Grid_CellPainting;
            _grid.CellMouseClick += Grid_CellMouseClick;

            Controls.Add(_grid);
            ResumeLayout(false);
        }

        private async void LoadData()
        {
            _grid.DataSource = await _repository.GetAllCitizensAsync();
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var form = new UserInfoForm();
            var controller = new UserInfoController(form, _repository);
            form.ShowDialog();
            LoadData();
        }

        // Vẽ 2 nút Sửa & Xóa nằm song song trong cột Thao tác
        private void Grid_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && _grid.Columns[e.ColumnIndex].Name == "colActions")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

                int padding = 4;
                int btnWidth = (e.CellBounds.Width - (padding * 3)) / 2;
                int btnHeight = e.CellBounds.Height - (padding * 2);

                Rectangle rectEdit = new Rectangle(e.CellBounds.X + padding, e.CellBounds.Y + padding, btnWidth, btnHeight);
                Rectangle rectDelete = new Rectangle(e.CellBounds.X + padding * 2 + btnWidth, e.CellBounds.Y + padding, btnWidth, btnHeight);

                // Vẽ nút Sửa
                ButtonRenderer.DrawButton(e.Graphics, rectEdit, "Sửa", _grid.Font, false, PushButtonState.Normal);

                // Vẽ nút Xóa
                ButtonRenderer.DrawButton(e.Graphics, rectDelete, "Xóa", _grid.Font, false, PushButtonState.Normal);

                e.Handled = true;
            }
        }

        // Bắt tọa độ click trong cell để phân biệt nút Sửa hay nút Xóa
        private async void Grid_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (_grid.Columns[e.ColumnIndex].Name == "colActions")
            {
                var list = (List<CitizenModel>)_grid.DataSource;
                if (list == null || e.RowIndex >= list.Count) return;

                var selectedItem = list[e.RowIndex];

                int padding = 4;
                Rectangle cellBounds = _grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                int btnWidth = (cellBounds.Width - (padding * 3)) / 2;
                int clickX = e.X; // Tọa độ click X tương đối trong ô

                // Click nút Sửa (Nửa bên trái)
                if (clickX >= padding && clickX <= padding + btnWidth)
                {
                    var form = new UserInfoForm(selectedItem);
                    var controller = new UserInfoController(form, _repository);
                    form.ShowDialog();
                    LoadData();
                }
                // Click nút Xóa (Nửa bên phải)
                else if (clickX >= padding * 2 + btnWidth && clickX <= padding * 2 + btnWidth * 2)
                {
                    var result = MessageBox.Show(
                        $"Bạn có chắc chắn muốn xóa nhân viên \"{selectedItem.FullName}\" không?",
                        "Xác nhận xóa",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        bool isDeleted = await _repository.DeleteCitizenAsync(selectedItem.Id);
                        if (isDeleted)
                        {
                            MessageBox.Show("Xóa nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                    }
                }
            }
        }
    }
}