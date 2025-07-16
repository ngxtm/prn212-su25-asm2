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
using BLL.Services;
using DAL.Entities;

namespace NguyenTheMinhWPF.Pages.Admin
{
    /// <summary>
    /// Interaction logic for ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        private readonly BookingReservationService _bookingReservationService;
        public ReportPage()
        {
            InitializeComponent();
            _bookingReservationService = new BookingReservationService();
        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (!dpStartDate.SelectedDate.HasValue || !dpEndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select both Start Date and End Date.");
                return;
            }
            var startDate = System.DateOnly.FromDateTime(dpStartDate.SelectedDate.Value);
            var endDate = System.DateOnly.FromDateTime(dpEndDate.SelectedDate.Value);
            if (startDate > endDate)
            {
                MessageBox.Show("Start Date must be before or equal to End Date.");
                return;
            }
            var reportData = _bookingReservationService.GetBookingReservationsByPeriod(startDate, endDate);
            dgReport.ItemsSource = reportData;
        }
    }
}
