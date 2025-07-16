using BLL.Services;
using DAL.Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NguyenTheMinhWPF.Pages.Customer
{
    public partial class ProfilePage : Page
    {
        private readonly CustomerService _customerService;
        private DAL.Entities.Customer _currentCustomer;

        public ProfilePage(DAL.Entities.Customer currentCustomer)
        {
            InitializeComponent();
            _customerService = new CustomerService();
            _currentCustomer = currentCustomer;
            LoadProfile();
        }

        private void LoadProfile()
        {
            txtFullName.Text = _currentCustomer.CustomerFullName;
            txtTelephone.Text = _currentCustomer.Telephone;
            txtEmail.Text = _currentCustomer.EmailAddress;
            if (_currentCustomer.CustomerBirthday.HasValue)
                dpBirthday.SelectedDate = _currentCustomer.CustomerBirthday.Value.ToDateTime(TimeOnly.MinValue);
        }

        private void btnUpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtTelephone.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }
            _currentCustomer.CustomerFullName = txtFullName.Text.Trim();
            _currentCustomer.Telephone = txtTelephone.Text.Trim();
            if (dpBirthday.SelectedDate.HasValue)
                _currentCustomer.CustomerBirthday = DateOnly.FromDateTime(dpBirthday.SelectedDate.Value);
            if (!string.IsNullOrWhiteSpace(pbPassword.Password))
                _currentCustomer.Password = pbPassword.Password;
            try
            {
                _customerService.UpdateCustomer(_currentCustomer);
                MessageBox.Show("Cập nhật hồ sơ thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật hồ sơ: {ex.Message}");
            }
        }
    }
} 