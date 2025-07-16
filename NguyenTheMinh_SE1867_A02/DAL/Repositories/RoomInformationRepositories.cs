using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class RoomInformationRepositories
    {
        private readonly FuminiHotelManagementContext _db;
        public RoomInformationRepositories()
        {
            _db = new FuminiHotelManagementContext();
        }

        public void AddRoom(RoomInformation roomInformation)
        {
            _db.RoomInformations.Add(roomInformation);
            _db.SaveChanges();
        }

        public string DeleteRoom(int id)
        {
            var room = _db.RoomInformations.Find(id);
            if (room is null) return "Không tìm thấy phòng với ID đã chọn";
            var isBooking = _db.BookingDetails.Any(b => b.RoomId == id);
            if (isBooking)
            {
                room.RoomStatus = 0;
                _db.SaveChanges();
                return "Không thể xoá phòng! Lý do: Phòng đang được booking. Đã chuyển trạng thái phòng sang INACTIVE";
            }
            _db.RoomInformations.Remove(room);
            _db.SaveChanges();
            return "Đã xoá phòng thành công!";
        }

        public List<RoomInformation> GetAllRoomInformations()
        {
            try
            {
                return _db.RoomInformations.Include(x => x.RoomType).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllRoomInformations: {ex.Message}");
                return new List<RoomInformation>();
            }
        }

        public List<RoomInformation> SearchRoom(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return _db.RoomInformations.ToList();
                }
                return _db.RoomInformations
                    .Where(r =>
                        (r.RoomNumber != null && r.RoomNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                        (r.RoomDetailDescription != null && r.RoomDetailDescription.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchRoom: {ex.Message}");
                return new List<RoomInformation>();
            }
        }

        public void UpdateRoom(RoomInformation selectedRoom)
        {
            _db.RoomInformations.Update(selectedRoom);
            _db.SaveChanges();
        }

        public List<RoomType> GetAllRoomTypes()
        {
            try
            {
                return _db.RoomTypes.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllRoomTypes: {ex.Message}");
                return new List<RoomType>();
            }
        }
    }
}