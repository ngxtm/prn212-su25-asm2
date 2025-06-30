using NguyenTheMinhWPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NguyenTheMinhWPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var email = txtEmail.Text.Trim();
            var password = pbPassword.Password;

            if (email == ConfigurationImport.AdminEmail &&
                password == ConfigurationImport.AdminPassword)
            {
                var adminDashboard = new AdminDashboard();
                adminDashboard.Show();
                this.Close();
                return;
            }
            else
            {
                MessageBox.Show("Email hoặc mật khẩu không đúng!");
            }
        }
    }
}
