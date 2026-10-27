using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
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
            Size = new Size(920, 560);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Surface;
            Font = new Font("Segoe UI", 10);

            // Header Panel
            var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Primary };
            pnlHeader.Controls.Add(new Label { Text = "DANH SÁCH NHÂN VIÊN", ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 16), AutoSize = true, Location = new Point(20, 18) });
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
                Size = new Size(864, 360),
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                RowHeadersVisible = false,
                RowTemplate = { Height = 35 }
            };

            // Cấu hình các cột hiển thị
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 50 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FullName", HeaderText = "Họ và tên", Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PersonalId", HeaderText = "CCCD", Width = 130 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PhoneNumber", HeaderText = "SĐT", Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "StartDate", HeaderText = "Ngày làm việc", Width = 110 });

            // Cột nút Sửa
            var editCol = new DataGridViewButtonColumn
            {
                HeaderText = "Thao tác",
                Text = "Sửa",
                UseColumnTextForButtonValue = true,
                Width = 50
            };
            _grid.Columns.Add(editCol);

            // Cột nút Xóa
            var deleteCol = new DataGridViewButtonColumn
            {
                HeaderText = "",
                Text = "Xóa",
                UseColumnTextForButtonValue = true,
                Width = 50
            };
            _grid.Columns.Add(deleteCol);

            _grid.CellContentClick += Grid_CellContentClick;
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
            LoadData(); // Tải lại danh sách sau khi đóng form thêm mới
        }

        private async void Grid_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var list = (List<CitizenModel>)_grid.DataSource;
            var selectedItem = list[e.RowIndex];

            // Click nút Sửa (Cột chỉ số 6)
            if (e.ColumnIndex == 6)
            {
                var form = new UserInfoForm(selectedItem);
                var controller = new UserInfoController(form, _repository);
                form.ShowDialog();
                LoadData(); // Tải lại danh sách sau khi sửa
            }
            // Click nút Xóa (Cột chỉ số 7)
            else if (e.ColumnIndex == 7)
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