using BLL.Services;
using System.Collections.Generic;
using System.Windows.Controls;
using DAL.Entities;

namespace NguyenTheMinhWPF.Pages.Customer
{
    public partial class BookingHistoryPage : Page
    {
        private readonly BookingReservationService _bookingReservationService;
        private readonly int _customerId;

        public BookingHistoryPage(int customerId)
        {
            InitializeComponent();
            _bookingReservationService = new BookingReservationService();
            _customerId = customerId;
            LoadBookingHistory();
        }

        private void LoadBookingHistory()
        {
            var allBookings = _bookingReservationService.GetAllBookingReservationsWithDetails();
            var customerBookings = allBookings.FindAll(b => b.CustomerID == _customerId);
            dgBookingHistory.ItemsSource = customerBookings;
        }
    }
} 