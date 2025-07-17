using BLL.Services;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace NguyenTheMinhWPF.Pages.Admin
{
    /// <summary>
    /// Interaction logic for BookingPage.xaml
    /// </summary>
    public partial class BookingPage : Page
    {
        private readonly BookingReservationService _bookingReservationService;
        private readonly CustomerService _customerService;
        private readonly RoomInformationService _roomService;
        private List<BookingReservationDisplayModel> _allBookings;
        private ObservableCollection<RoomDetailViewModel> RoomDetails = new ObservableCollection<RoomDetailViewModel>();

        public BookingPage()
        {
            InitializeComponent();
            _bookingReservationService = new BookingReservationService();
            _customerService = new CustomerService();
            _roomService = new RoomInformationService();
            dgRoomDetails.ItemsSource = RoomDetails;
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBookings();
        }

        private void LoadBookings()
        {
            _allBookings = _bookingReservationService.GetAllBookingReservationsWithDetails();
            dgBooking.ItemsSource = _allBookings;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(keyword))
            {
                dgBooking.ItemsSource = _allBookings;
            }
            else
            {
                dgBooking.ItemsSource = _allBookings.Where(b =>
                    (b.CustomerFullName != null && b.CustomerFullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    (b.RoomNumber != null && b.RoomNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                    b.BookingReservationID.ToString().Contains(keyword) ||
                    b.CustomerID.ToString().Contains(keyword) ||
                    b.RoomID.ToString().Contains(keyword)
                ).ToList();
            }
        }

        private void btnAddRoom_Click(object sender, RoutedEventArgs e)
        {
            var addRoomDialog = new AddRoomDialog(_roomService.GetAllRoomInformations());
            if (addRoomDialog.ShowDialog() == true)
            {
                var selectedRoom = addRoomDialog.SelectedRoom;
                var startDate = addRoomDialog.StartDate;
                var endDate = addRoomDialog.EndDate;
                
                var startDateOnly = DateOnly.FromDateTime(startDate);
                var endDateOnly = DateOnly.FromDateTime(endDate);
                
                if (!_bookingReservationService.IsRoomAvailable(selectedRoom.RoomId, startDateOnly, endDateOnly))
                {
                    MessageBox.Show($"Room {selectedRoom.RoomNumber} is not available for the selected date range ({startDateOnly:yyyy-MM-dd} to {endDateOnly:yyyy-MM-dd})!");
                    return;
                }

                decimal actualPrice = selectedRoom.RoomPricePerDay.HasValue ? selectedRoom.RoomPricePerDay.Value * ((endDate - startDate).Days + 1) : 0;
                RoomDetails.Add(new RoomDetailViewModel
                {
                    RoomID = selectedRoom.RoomId,
                    RoomNumber = selectedRoom.RoomNumber,
                    StartDate = startDate,
                    EndDate = endDate,
                    ActualPrice = actualPrice
                });
                UpdateTotalPrice();
            }
        }

        private void btnRemoveRoom_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is RoomDetailViewModel roomDetail)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xoá phòng này khỏi đặt phòng không?", "Xoá phòng", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    RoomDetails.Remove(roomDetail);
                    UpdateTotalPrice();
                }
            }
        }

        private void UpdateTotalPrice()
        {
            txtTotalPrice.Text = RoomDetails.Sum(r => r.ActualPrice).ToString("F2");
        }

        private void dgRoomDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            UpdateTotalPrice();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new BookingDialog();
                if (dialog.ShowDialog() == true)
                {
                    _bookingReservationService.AddBookingWithDetails(dialog.Booking, dialog.RoomDetails);
                    MessageBox.Show("Thêm đặt phòng thành công!");
                    LoadBookings();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Validation error: {ex.Message}", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgBooking.SelectedItem is BookingReservationDisplayModel selectedBooking)
                {
                    var booking = new BookingReservation
                    {
                        BookingReservationId = selectedBooking.BookingReservationID,
                        BookingDate = selectedBooking.BookingDate,
                        CustomerId = selectedBooking.CustomerID,
                        BookingStatus = selectedBooking.BookingStatus
                    };
                    var dialog = new BookingDialog(booking, new List<BookingDetail>());
                    if (dialog.ShowDialog() == true)
                    {
                        _bookingReservationService.UpdateBookingWithDetailsSmart(dialog.Booking, dialog.RoomDetails);
                        MessageBox.Show("Cập nhật đặt phòng thành công!");
                        LoadBookings();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn đặt phòng để cập nhật!");
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Validation error: {ex.Message}", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgBooking.SelectedItem is BookingReservationDisplayModel selectedBooking)
                {
                    var result = MessageBox.Show("Bạn có muốn xoá lịch đặt phòng này không?", "Xoá lịch đặt phòng", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        _bookingReservationService.DeleteBooking(selectedBooking.BookingReservationID, selectedBooking.RoomID);
                        MessageBox.Show("Xoá đặt phòng thành công!");
                        ClearForm();
                        LoadBookings();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một bản ghi để xoá!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xoá đặt phòng: {ex.Message}");
            }
        }

        private void dgBooking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBooking.SelectedItem is BookingReservationDisplayModel model)
            {
                FillForm(model);
            }
            else
            {
                ClearForm();
            }
        }

        private void FillForm(BookingReservationDisplayModel model)
        {
            txtBookingReservationID.Text = model.BookingReservationID.ToString();
            dpBookingDate.SelectedDate = model.BookingDate?.ToDateTime(TimeOnly.MinValue);
            txtTotalPrice.Text = model.TotalPrice?.ToString();
            txtCustomerID.Text = model.CustomerID.ToString();
            txtCustomerFullName.Text = model.CustomerFullName;
            txtTelephone.Text = model.Telephone;
            
            LoadRoomDetailsForBooking(model.BookingReservationID);
        }

        private void LoadRoomDetailsForBooking(int bookingId)
        {
            RoomDetails.Clear();
            var bookingDetails = _allBookings.Where(b => b.BookingReservationID == bookingId);
            foreach (var detail in bookingDetails)
            {
                RoomDetails.Add(new RoomDetailViewModel
                {
                    RoomID = detail.RoomID,
                    RoomNumber = detail.RoomNumber,
                    StartDate = detail.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = detail.EndDate.ToDateTime(TimeOnly.MinValue),
                    ActualPrice = detail.ActualPrice ?? 0
                });
            }
            UpdateTotalPrice();
        }

        private void txtCustomerID_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtCustomerError.Visibility = Visibility.Collapsed;
        }

        private void ClearForm()
        {
            txtBookingReservationID.Text = string.Empty;
            dpBookingDate.SelectedDate = null;
            txtTotalPrice.Text = string.Empty;
            txtCustomerID.Text = string.Empty;
            txtCustomerFullName.Text = string.Empty;
            txtTelephone.Text = string.Empty;
            dgBooking.SelectedItem = null;
            txtCustomerError.Visibility = Visibility.Collapsed;
            RoomDetails.Clear();
        }

        private bool ValidateForm()
        {
            bool valid = true;
            txtCustomerError.Visibility = Visibility.Collapsed;
            if (!int.TryParse(txtCustomerID.Text, out int customerId) || _customerService.GetAllCustomers().FirstOrDefault(c => c.CustomerId == customerId) == null)
            {
                txtCustomerError.Text = "Customer ID không hợp lệ hoặc không tồn tại!";
                txtCustomerError.Visibility = Visibility.Visible;
                valid = false;
            }
            if (RoomDetails.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất một phòng!");
                valid = false;
            }
            return valid;
        }

        private void btnLookupCustomer_Click(object sender, RoutedEventArgs e)
        {
            txtCustomerError.Visibility = Visibility.Collapsed;
            if (int.TryParse(txtCustomerID.Text, out int customerId))
            {
                var customer = _customerService.GetAllCustomers().FirstOrDefault(c => c.CustomerId == customerId);
                if (customer != null)
                {
                    txtCustomerFullName.Text = customer.CustomerFullName;
                    txtTelephone.Text = customer.Telephone;
                }
                else
                {
                    txtCustomerFullName.Text = string.Empty;
                    txtTelephone.Text = string.Empty;
                    txtCustomerError.Text = "Không tìm thấy khách hàng!";
                    txtCustomerError.Visibility = Visibility.Visible;
                }
            }
            else
            {
                txtCustomerFullName.Text = string.Empty;
                txtTelephone.Text = string.Empty;
                txtCustomerError.Text = "Customer ID không hợp lệ!";
                txtCustomerError.Visibility = Visibility.Visible;
            }
        }
    }

    public class RoomDetailViewModel
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal ActualPrice { get; set; }
    }
}
