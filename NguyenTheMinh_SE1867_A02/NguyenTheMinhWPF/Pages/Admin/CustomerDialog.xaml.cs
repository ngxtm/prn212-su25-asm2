using DAL.Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NguyenTheMinhWPF.Pages.Admin
{
    public partial class CustomerDialog : Window
    {
        public DAL.Entities.Customer Customer { get; private set; }
        public CustomerDialog(DAL.Entities.Customer customer = null)
        {
            InitializeComponent();
            if (customer != null)
            {
                txtFullName.Text = customer.CustomerFullName;
                txtTelephone.Text = customer.Telephone;
                txtEmail.Text = customer.EmailAddress;
                if (customer.CustomerBirthday.HasValue)
                    dpBirthday.SelectedDate = customer.CustomerBirthday.Value.ToDateTime(TimeOnly.MinValue);
                cbStatus.SelectedIndex = customer.CustomerStatus == 1 ? 0 : 1;
                txtPassword.Text = customer.Password;
                Customer = customer;
            }
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtTelephone.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || !dpBirthday.SelectedDate.HasValue || cbStatus.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }
            if (Customer == null) Customer = new DAL.Entities.Customer();
            Customer.CustomerFullName = txtFullName.Text.Trim();
            Customer.Telephone = txtTelephone.Text.Trim();
            Customer.EmailAddress = txtEmail.Text.Trim();
            Customer.CustomerBirthday = DateOnly.FromDateTime(dpBirthday.SelectedDate.Value);
            Customer.CustomerStatus = Convert.ToByte(((ComboBoxItem)cbStatus.SelectedItem).Tag);
            Customer.Password = txtPassword.Text;
            DialogResult = true;
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 