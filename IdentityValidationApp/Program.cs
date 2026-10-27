using System;
using System.Windows.Forms;
using IdentityValidationApp.Views;
using IdentityValidationApp.Repositories;

namespace IdentityValidationApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Cập nhật chuỗi kết nối MySQL khớp với máy tính của bạn
            string connectionString = "Server=127.0.0.1;Port=3306;Database=IdentityValidationDb;User=root;Password=root;";
            var repository = new CitizenRepository(connectionString);

            // Khởi chạy ứng dụng từ Trang chủ Danh sách Nhân viên
            Application.Run(new MainListForm(repository));
        }
    }
}