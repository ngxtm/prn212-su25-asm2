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
    /// Interaction logic for RoomPage.xaml
    /// </summary>
    public partial class RoomPage : Page
    {
        private readonly RoomInformationService _roomInformationService;

        public RoomPage()
        {
            InitializeComponent();
            this._roomInformationService = new RoomInformationService();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadRoomType();
                var rooms = _roomInformationService.GetAllRoomInformations();
                if (rooms != null)
                {
                    dgRoom.ItemsSource = rooms;
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu phòng!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            txtRoomNumber.Text = "";
            txtRoomDetailDescription.Text = "";
            txtMaxCapacity = null;
            cbRoomType.SelectedIndex = -1;
            cbStatus.SelectedIndex = -1;
            txtPrice.Text = "";
        }

        private void AutoFillInputFields(RoomInformation room)
        {
            txtRoomNumber.Text = room.RoomNumber ?? string.Empty;
            txtRoomDetailDescription.Text = room.RoomDetailDescription ?? string.Empty;
            txtMaxCapacity.Text = room.RoomMaxCapacity?.ToString() ?? string.Empty;
            cbRoomType.SelectedValue = room.RoomType?.RoomTypeId;
            cbStatus.SelectedValue = room.RoomStatus;
            txtPrice.Text = room.RoomPricePerDay?.ToString("F2") ?? string.Empty;
        }

        private void LoadRoomType()
        {
            var roomTypes = _roomInformationService.GetAllRoomTypes();
            cbRoomType.ItemsSource = roomTypes;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgRoom.ItemsSource = _roomInformationService.SearchRoom(txtSearch.Text ?? string.Empty);
            Page_Loaded(sender, e);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ mã phòng!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtRoomDetailDescription.Text))
                {
                    MessageBox.Show("Vui lòng nhập thông tin chi tiết của phòng!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMaxCapacity.Text))
                {
                    MessageBox.Show("Vui lòng nhập sức chứa tối đa của phòng!");
                    return;
                }

                if (cbRoomType.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn loại phòng!");
                    return;
                }

                if (cbStatus.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn trạng thái!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPrice.Text))
                {
                    MessageBox.Show("Vui lòng nhập giá phòng tính theo ngày!");
                    return;
                }

                var roomInformation = new RoomInformation
                {
                    RoomNumber = txtRoomNumber.Text.Trim(),
                    RoomDetailDescription = txtRoomDetailDescription.Text.Trim(),
                    RoomMaxCapacity = Convert.ToInt32(txtMaxCapacity.Text.Trim()),
                    RoomTypeId = Convert.ToInt32(cbRoomType.SelectedValue),
                    RoomStatus = Convert.ToByte(cbStatus.SelectedValue),
                    RoomPricePerDay = Convert.ToDecimal(txtPrice.Text.Trim())
                };

                _roomInformationService.AddRoom(roomInformation);
                MessageBox.Show("Thêm phòng thành công!");

                ClearForm();

                Page_Loaded(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm phòng: {ex.Message}");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
            {
                MessageBox.Show("Vui lòng nhập họ mã phòng!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRoomDetailDescription.Text))
            {
                MessageBox.Show("Vui lòng nhập thông tin chi tiết của phòng!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMaxCapacity.Text))
            {
                MessageBox.Show("Vui lòng nhập sức chứa tối đa của phòng!");
                return;
            }

            if (cbRoomType.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại phòng!");
                return;
            }

            if (cbStatus.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn trạng thái!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Vui lòng nhập giá phòng tính theo ngày!");
                return;
            }

            if (dgRoom.SelectedItem is RoomInformation selectedRoom)
            {
                try
                {
                    selectedRoom.RoomNumber = txtRoomNumber.Text.Trim();
                    selectedRoom.RoomDetailDescription = txtRoomDetailDescription.Text.Trim();
                    selectedRoom.RoomMaxCapacity = Convert.ToInt32(txtMaxCapacity.Text.Trim());
                    selectedRoom.RoomTypeId = Convert.ToInt32(cbRoomType.SelectedValue);
                    selectedRoom.RoomStatus = Convert.ToByte(cbStatus.SelectedValue);
                    selectedRoom.RoomPricePerDay = Convert.ToDecimal(txtPrice.Text.Trim());
                    _roomInformationService.UpdateRoom(selectedRoom);
                    Page_Loaded(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật phòng: {ex.Message}");
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedRoom = dgRoom.SelectedItem as RoomInformation;
            int id = selectedRoom?.RoomId ?? 0;
            if (selectedRoom == null)
            {
                MessageBox.Show("Vui lòng chọn phòng để xóa!");
                return;
            }
            if (id == 0)
            {
                MessageBox.Show("ID phòng không hợp lệ!");
                return;
            }
            var message = _roomInformationService.DeleteRoom(id);
            MessageBox.Show(message);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void dgRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgRoom.SelectedItem is RoomInformation selectdRoom)
            {
                AutoFillInputFields(selectdRoom);
            }
            else
            {
                ClearForm();
            }
        }
    }
}
