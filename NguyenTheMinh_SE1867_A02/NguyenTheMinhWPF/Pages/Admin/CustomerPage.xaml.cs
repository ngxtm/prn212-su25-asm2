using BLL.Services;
using DAL.Entities;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NguyenTheMinhWPF.Pages.Admin
{
    /// <summary>
    /// Interaction logic for CustomerPage.xaml
    /// </summary>
    public partial class CustomerPage : Page
    {
        private readonly CustomerService _customerService;
        public CustomerPage()
        {
            InitializeComponent();
            this._customerService = new CustomerService();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                if (customers != null)
                {
                    dgCustomer.ItemsSource = customers;
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu khách hàng!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text ?? string.Empty;
            dgCustomer.ItemsSource = _customerService.SearchCustomer(keyword);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ tên khách hàng!");
                    return;
                }

                var telephone = txtTelephone.Text;
                if (string.IsNullOrWhiteSpace(telephone) ||
                    !System.Text.RegularExpressions.Regex.IsMatch(telephone, @"^(?:\+84|0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-5]|9[0-9])[0-9]{7}$"))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ vui lòng nhập lại");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Vui lòng nhập email!");
                    return;
                }

                if (!dpBirthday.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn ngày sinh!");
                    return;
                }

                if (cbStatus.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn trạng thái!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu!");
                    return;
                }

                var customer = new DAL.Entities.Customer
                {
                    CustomerFullName = txtFullName.Text.Trim(),
                    Telephone = telephone.Trim(),
                    EmailAddress = txtEmail.Text.Trim(),
                    CustomerBirthday = DateOnly.FromDateTime(dpBirthday.SelectedDate.Value),
                    CustomerStatus = Convert.ToByte(cbStatus.SelectedValue),
                    Password = txtPassword.Text
                };

                _customerService.AddCustomer(customer);
                MessageBox.Show("Thêm khách hàng thành công!");
                
                ClearForm();
                
                Page_Loaded(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm khách hàng: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            txtFullName.Text = "";
            txtTelephone.Text = "";
            txtEmail.Text = "";
            dpBirthday.SelectedDate = null;
            cbStatus.SelectedIndex = -1;
            txtPassword.Text = "";
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên khách hàng!");
                return;
            }

            var telephone = txtTelephone.Text;
            if (string.IsNullOrWhiteSpace(telephone) ||
                !System.Text.RegularExpressions.Regex.IsMatch(telephone, @"^(?:\+84|0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-5]|9[0-9])[0-9]{7}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ vui lòng nhập lại");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập email!");
                return;
            }

            if (!dpBirthday.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày sinh!");
                return;
            }

            if (cbStatus.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn trạng thái!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!");
                return;
            }

            if (dgCustomer.SelectedItem is DAL.Entities.Customer customer)
            {
                try
                {
                    customer.CustomerFullName = txtFullName.Text.Trim();
                    customer.Telephone = telephone;
                    customer.EmailAddress = txtEmail.Text;
                    customer.CustomerBirthday = DateOnly.FromDateTime(dpBirthday.SelectedDate.Value);
                    customer.CustomerStatus = Convert.ToByte(cbStatus.SelectedValue);
                    customer.Password = txtPassword.Text;
                    Page_Loaded(sender, e);
                    MessageBox.Show($"Đã update khách hàng id: {customer.CustomerId} thành công!");
                } catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while updating the project: {ex.Message}");
                    return;
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn xoá khách hàng này không?", "Xác nhận xoá", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                if (dgCustomer.SelectedItem is DAL.Entities.Customer customer)
                {
                    int id = customer.CustomerId;
                    _customerService.DeleteCustomer(id);
                    Page_Loaded(sender, e);
                }
            }    
        }

        private void dgCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCustomer.SelectedItem is DAL.Entities.Customer customer)
            {
                AutoFillInputFields(customer);
            }
            else
            {
                ClearForm();
            }
        }

        private void AutoFillInputFields(DAL.Entities.Customer customer)
        {
            txtFullName.Text = customer.CustomerFullName;
            txtTelephone.Text = customer.Telephone;
            txtEmail.Text = customer.EmailAddress;
            dpBirthday.SelectedDate = customer.CustomerBirthday.HasValue ? customer.CustomerBirthday.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null;
            cbStatus.SelectedValue = customer.CustomerStatus;
            txtPassword.Text = customer.Password;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
    }
}
