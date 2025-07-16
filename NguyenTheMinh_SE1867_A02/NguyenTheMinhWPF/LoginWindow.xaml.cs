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
using BLL.Services;
using DAL.Entities;
using NguyenTheMinhWPF.Pages.Customer;

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
                var customerService = new CustomerService();
                var customer = customerService.GetCustomerByEmailAndPassword(email, password);
                if (customer != null)
                {
                    var customerDashboard = new CustomerDashboard(customer);
                    customerDashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Email hoặc mật khẩu không đúng!");
                }
            }
        }
    }
}
