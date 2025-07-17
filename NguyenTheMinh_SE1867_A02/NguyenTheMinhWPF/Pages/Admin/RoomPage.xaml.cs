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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            dgRoom.ItemsSource = _roomInformationService.SearchRoom(txtSearch.Text ?? string.Empty);
            Page_Loaded(sender, e);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new RoomDialog(_roomInformationService.GetAllRoomTypes());
            if (dialog.ShowDialog() == true)
            {
                _roomInformationService.AddRoom(dialog.Room);
                MessageBox.Show("Thêm phòng thành công!");
                Page_Loaded(sender, e);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgRoom.SelectedItem is RoomInformation selectedRoom)
            {
                var dialog = new RoomDialog(_roomInformationService.GetAllRoomTypes(), selectedRoom);
                if (dialog.ShowDialog() == true)
                {
                    _roomInformationService.UpdateRoom(dialog.Room);
                    MessageBox.Show("Cập nhật phòng thành công!");
                    Page_Loaded(sender, e);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn phòng để cập nhật!");
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
            var result = MessageBox.Show("Bạn có chắc chắn muốn xoá phòng này hay không!", "Xác nhận", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                string message = _roomInformationService.DeleteRoom(id);
                MessageBox.Show(message);
            }    
        }
    }
}
