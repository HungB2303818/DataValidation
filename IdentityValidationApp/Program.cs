using System;
using System.Windows.Forms;
using IdentityValidationApp.Views;
using IdentityValidationApp.Controllers;
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

            // Cập nhật chuỗi kết nối MySQL khớp với máy của bạn
            string connectionString = "Server=127.0.0.1;Port=3306;Database=IdentityValidationDb;User=root;Password=your_password;";
            var repository = new CitizenRepository(connectionString);

            var form = new UserInfoForm();
            var controller = new UserInfoController(form, repository);

            Application.Run(form);
        }
    }
}