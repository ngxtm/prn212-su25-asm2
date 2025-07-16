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
    /// Interaction logic for BookingPage.xaml
    /// </summary>
    public partial class BookingPage : Page
    {
        private readonly BookingReservationService _bookingReservationService;
        private readonly CustomerService _customerService;
        private readonly RoomInformationService _roomService;
        private List<BookingReservationDisplayModel> _allBookings;

        public BookingPage()
        {
            InitializeComponent();
            _bookingReservationService = new BookingReservationService();
            _customerService = new CustomerService();
            _roomService = new RoomInformationService();
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            try
            {
                var model = GetModelFromForm();
                _bookingReservationService.AddBooking(model);
                MessageBox.Show("Thêm đặt phòng thành công!");
                ClearForm();
                LoadBookings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm đặt phòng: {ex.Message}");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            try
            {
                var model = GetModelFromForm();
                _bookingReservationService.UpdateBooking(model);
                MessageBox.Show("Cập nhật đặt phòng thành công!");
                ClearForm();
                LoadBookings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật đặt phòng: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(txtBookingReservationID.Text, out int bookingId) && int.TryParse(txtRoomID.Text, out int roomId))
                {
                    _bookingReservationService.DeleteBooking(bookingId, roomId);
                    MessageBox.Show("Xoá đặt phòng thành công!");
                    ClearForm();
                    LoadBookings();
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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
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

        private BookingReservationDisplayModel GetModelFromForm()
        {
            return new BookingReservationDisplayModel
            {
                BookingReservationID = int.TryParse(txtBookingReservationID.Text, out int bid) ? bid : 0,
                BookingDate = dpBookingDate.SelectedDate.HasValue ? System.DateOnly.FromDateTime(dpBookingDate.SelectedDate.Value) : null,
                TotalPrice = decimal.TryParse(txtTotalPrice.Text, out decimal tp) ? tp : null,
                CustomerID = int.TryParse(txtCustomerID.Text, out int cid) ? cid : 0,
                CustomerFullName = txtCustomerFullName.Text,
                Telephone = txtTelephone.Text,
                BookingStatus = cbBookingStatus.SelectedValue is byte bstatus ? bstatus : (cbBookingStatus.SelectedItem is ComboBoxItem item ? Convert.ToByte(item.Tag) : (byte?)null),
                RoomID = int.TryParse(txtRoomID.Text, out int rid) ? rid : 0,
                RoomNumber = txtRoomNumber.Text,
                StartDate = dpStartDate.SelectedDate.HasValue ? System.DateOnly.FromDateTime(dpStartDate.SelectedDate.Value) : default,
                EndDate = dpEndDate.SelectedDate.HasValue ? System.DateOnly.FromDateTime(dpEndDate.SelectedDate.Value) : default,
                ActualPrice = decimal.TryParse(txtActualPrice.Text, out decimal ap) ? ap : null
            };
        }

        private void FillForm(BookingReservationDisplayModel model)
        {
            txtBookingReservationID.Text = model.BookingReservationID.ToString();
            dpBookingDate.SelectedDate = model.BookingDate?.ToDateTime(TimeOnly.MinValue);
            txtTotalPrice.Text = model.TotalPrice?.ToString();
            txtCustomerID.Text = model.CustomerID.ToString();
            txtCustomerFullName.Text = model.CustomerFullName;
            txtTelephone.Text = model.Telephone;
            if (model.BookingStatus.HasValue)
            {
                foreach (ComboBoxItem item in cbBookingStatus.Items)
                {
                    if (item.Tag != null && item.Tag.ToString() == model.BookingStatus.Value.ToString())
                    {
                        cbBookingStatus.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                cbBookingStatus.SelectedIndex = -1;
            }
            txtRoomID.Text = model.RoomID.ToString();
            txtRoomNumber.Text = model.RoomNumber;
            dpStartDate.SelectedDate = model.StartDate.ToDateTime(TimeOnly.MinValue);
            dpEndDate.SelectedDate = model.EndDate.ToDateTime(TimeOnly.MinValue);
            txtActualPrice.Text = model.ActualPrice?.ToString();
        }

        private void txtCustomerID_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtCustomerError.Visibility = Visibility.Collapsed;
        }

        private void txtRoomID_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtRoomError.Visibility = Visibility.Collapsed;
        }

        private void ClearForm()
        {
            txtBookingReservationID.Text = "";
            dpBookingDate.SelectedDate = null;
            txtTotalPrice.Text = "";
            txtCustomerID.Text = "";
            txtCustomerFullName.Text = "";
            txtTelephone.Text = "";
            cbBookingStatus.SelectedIndex = -1;
            txtRoomID.Text = "";
            txtRoomNumber.Text = "";
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            txtActualPrice.Text = "";
            dgBooking.SelectedItem = null;
            txtCustomerError.Visibility = Visibility.Collapsed;
            txtRoomError.Visibility = Visibility.Collapsed;
        }

        private bool ValidateForm()
        {
            bool valid = true;
            txtCustomerError.Visibility = Visibility.Collapsed;
            txtRoomError.Visibility = Visibility.Collapsed;

            if (!int.TryParse(txtCustomerID.Text, out int customerId) || _customerService.GetAllCustomers().FirstOrDefault(c => c.CustomerId == customerId) == null)
            {
                txtCustomerError.Text = "Customer ID không hợp lệ hoặc không tồn tại!";
                txtCustomerError.Visibility = Visibility.Visible;
                valid = false;
            }
            if (!int.TryParse(txtRoomID.Text, out int roomId) || _roomService.GetAllRoomInformations().FirstOrDefault(r => r.RoomId == roomId) == null)
            {
                txtRoomError.Text = "Room ID không hợp lệ hoặc không tồn tại!";
                txtRoomError.Visibility = Visibility.Visible;
                valid = false;
            }
            if (string.IsNullOrWhiteSpace(txtTotalPrice.Text) || !decimal.TryParse(txtTotalPrice.Text, out _))
            {
                MessageBox.Show("TotalPrice không hợp lệ!");
                valid = false;
            }
            if (!dpBookingDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn BookingDate!");
                valid = false;
            }
            if (!dpStartDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn StartDate!");
                valid = false;
            }
            if (!dpEndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn EndDate!");
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
                    txtCustomerFullName.Text = "";
                    txtTelephone.Text = "";
                    txtCustomerError.Text = "Không tìm thấy khách hàng!";
                    txtCustomerError.Visibility = Visibility.Visible;
                }
            }
            else
            {
                txtCustomerFullName.Text = "";
                txtTelephone.Text = "";
                txtCustomerError.Text = "Customer ID không hợp lệ!";
                txtCustomerError.Visibility = Visibility.Visible;
            }
        }

        private void btnLookupRoom_Click(object sender, RoutedEventArgs e)
        {
            txtRoomError.Visibility = Visibility.Collapsed;
            if (int.TryParse(txtRoomID.Text, out int roomId))
            {
                var room = _roomService.GetAllRoomInformations().FirstOrDefault(r => r.RoomId == roomId);
                if (room != null)
                {
                    txtRoomNumber.Text = room.RoomNumber;
                }
                else
                {
                    txtRoomNumber.Text = "";
                    txtRoomError.Text = "Không tìm thấy phòng!";
                    txtRoomError.Visibility = Visibility.Visible;
                }
            }
            else
            {
                txtRoomNumber.Text = "";
                txtRoomError.Text = "Room ID không hợp lệ!";
                txtRoomError.Visibility = Visibility.Visible;
            }
        }
    }
}
