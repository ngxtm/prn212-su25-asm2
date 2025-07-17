using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NguyenTheMinhWPF.Pages.Admin
{
    public partial class AddRoomDialog : Window
    {
        public RoomInformation SelectedRoom { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        private List<RoomInformation> _rooms;

        public AddRoomDialog(List<RoomInformation> rooms)
        {
            InitializeComponent();
            _rooms = rooms;
            cbRooms.ItemsSource = _rooms;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (cbRooms.SelectedItem is RoomInformation room && dpStartDate.SelectedDate.HasValue && dpEndDate.SelectedDate.HasValue)
            {
                // Validate that start date is before end date
                if (dpStartDate.SelectedDate.Value >= dpEndDate.SelectedDate.Value)
                {
                    MessageBox.Show("Start date must be before end date!");
                    return;
                }

                SelectedRoom = room;
                StartDate = dpStartDate.SelectedDate.Value;
                EndDate = dpEndDate.SelectedDate.Value;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a room and both dates.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 