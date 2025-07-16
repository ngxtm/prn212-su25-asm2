using System.Windows;
using System.Windows.Controls;
using DAL.Entities;

namespace NguyenTheMinhWPF.Pages.Customer
{
    public partial class CustomerDashboard : Window
    {
        private readonly DAL.Entities.Customer _currentCustomer;
        public CustomerDashboard(DAL.Entities.Customer customer)
        {
            InitializeComponent();
            _currentCustomer = customer;
            MainFrame.Content = new ProfilePage(_currentCustomer);
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfilePage(_currentCustomer));
        }

        private void btnBookingHistory_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BookingHistoryPage(_currentCustomer.CustomerId));
        }
    }
} 