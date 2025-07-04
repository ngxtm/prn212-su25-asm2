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
            throw new NotImplementedException();
        }

        public void DeleteRoom(int id)
        {
            throw new NotImplementedException();
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
    }
}