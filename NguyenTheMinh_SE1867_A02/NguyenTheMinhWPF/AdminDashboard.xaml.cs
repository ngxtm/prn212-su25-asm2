using NguyenTheMinhWPF.Pages.Admin;
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
using System.Windows.Navigation;

namespace NguyenTheMinhWPF
{
    /// <summary>
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
    public partial class AdminDashboard : Window
    {

        private readonly Lazy<CustomerPage> _customerPage = new(() => new CustomerPage());
        private readonly Lazy<RoomPage> _roomPage = new(() => new RoomPage());
        private readonly Lazy<BookingPage> _bookingPage = new(() => new BookingPage());
        private readonly Lazy<ReportPage> _reportPage = new(() => new ReportPage());

        public AdminDashboard()
        {
            InitializeComponent();
            MainFrame.Content = new CustomerPage();
        }

        private void btnCustomer_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_customerPage.Value);
        }

        private void btnRoom_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_roomPage.Value);
        }

        private void btnBooking_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_bookingPage.Value);
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_reportPage.Value);
        }
    }
}
