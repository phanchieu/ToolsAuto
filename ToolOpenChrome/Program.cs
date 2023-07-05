using System.Diagnostics;
using System.Security.Principal;

namespace ToolOpenChrome
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            if (IsRunAsAdmin())
            {
                ApplicationConfiguration.Initialize();
                LoginForm loginForm = new LoginForm();

                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new FormMain());
                }
            }
            else
            {
                MessageBox.Show("Ứng dụng cần chạy với quyền quản trị.", "Cảnh báo!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }

            //Application.Run(new Form1());
        }
        private static bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}