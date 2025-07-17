using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NguyenTheMinhWPF.Pages.Admin
{
    public partial class BookingDialog : Window
    {
        public BookingReservation Booking { get; private set; }
        public List<BookingDetail> RoomDetails { get; private set; }
        public BookingDialog(BookingReservation booking = null, List<BookingDetail> details = null)
        {
            InitializeComponent();
            if (booking != null)
            {
                dpBookingDate.SelectedDate = booking.BookingDate?.ToDateTime(TimeOnly.MinValue);
                txtCustomerID.Text = booking.CustomerId.ToString();
                cbBookingStatus.SelectedIndex = booking.BookingStatus == 1 ? 0 : 1;
                Booking = booking;
            }
            RoomDetails = details ?? new List<BookingDetail>();
            lbRoomDetails.ItemsSource = RoomDetails;
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!dpBookingDate.SelectedDate.HasValue || string.IsNullOrWhiteSpace(txtCustomerID.Text) || cbBookingStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }
            if (Booking == null) Booking = new BookingReservation();
            Booking.BookingDate = DateOnly.FromDateTime(dpBookingDate.SelectedDate.Value);
            Booking.CustomerId = int.Parse(txtCustomerID.Text.Trim());
            Booking.BookingStatus = Convert.ToByte(((ComboBoxItem)cbBookingStatus.SelectedItem).Tag);
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