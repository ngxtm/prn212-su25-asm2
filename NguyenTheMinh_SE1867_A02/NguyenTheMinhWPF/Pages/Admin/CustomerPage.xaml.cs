using BLL.Services;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            Loaded += Page_Loaded;
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
            var dialog = new CustomerDialog();
            if (dialog.ShowDialog() == true)
            {
                _customerService.AddCustomer(dialog.Customer);
                MessageBox.Show("Thêm khách hàng thành công!");
                Page_Loaded(sender, e);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomer.SelectedItem is DAL.Entities.Customer customer)
            {
                var dialog = new CustomerDialog(customer);
                if (dialog.ShowDialog() == true)
                {
                    _customerService.UpdateCustomer(dialog.Customer);
                    MessageBox.Show($"Đã update khách hàng id: {customer.CustomerId} thành công!");
                    Page_Loaded(sender, e);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn khách hàng để cập nhật!");
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
    }
}
