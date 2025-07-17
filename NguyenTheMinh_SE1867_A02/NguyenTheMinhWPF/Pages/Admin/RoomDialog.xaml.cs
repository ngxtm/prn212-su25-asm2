using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NguyenTheMinhWPF.Pages.Admin
{
    public partial class RoomDialog : Window
    {
        public RoomInformation Room { get; private set; }
        public RoomDialog(List<RoomType> roomTypes, RoomInformation room = null)
        {
            InitializeComponent();
            cbRoomType.ItemsSource = roomTypes;
            if (room != null)
            {
                txtRoomNumber.Text = room.RoomNumber;
                txtRoomDetailDescription.Text = room.RoomDetailDescription;
                txtMaxCapacity.Text = room.RoomMaxCapacity?.ToString();
                cbRoomType.SelectedValue = room.RoomTypeId;
                cbStatus.SelectedIndex = room.RoomStatus == 1 ? 0 : 1;
                txtPrice.Text = room.RoomPricePerDay?.ToString();
                Room = room;
            }
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text) || string.IsNullOrWhiteSpace(txtRoomDetailDescription.Text) || string.IsNullOrWhiteSpace(txtMaxCapacity.Text) || cbRoomType.SelectedValue == null || cbStatus.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }
            if (Room == null) Room = new RoomInformation();
            Room.RoomNumber = txtRoomNumber.Text.Trim();
            Room.RoomDetailDescription = txtRoomDetailDescription.Text.Trim();
            Room.RoomMaxCapacity = Convert.ToInt32(txtMaxCapacity.Text.Trim());
            Room.RoomTypeId = Convert.ToInt32(cbRoomType.SelectedValue);
            Room.RoomStatus = Convert.ToByte(((ComboBoxItem)cbStatus.SelectedItem).Tag);
            Room.RoomPricePerDay = Convert.ToDecimal(txtPrice.Text.Trim());
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