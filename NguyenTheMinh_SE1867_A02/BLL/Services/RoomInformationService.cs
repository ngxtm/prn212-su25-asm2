using DAL.Entities;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class RoomInformationService
    {
        private readonly RoomInformationRepositories _roomInformationRepositories;
        public RoomInformationService()
        {
            this._roomInformationRepositories = new RoomInformationRepositories();
        }
        public List<RoomInformation> GetAllRoomInformations()
        {
            return _roomInformationRepositories.GetAllRoomInformations();
        }
        public List<RoomInformation> SearchRoom(string keyword)
        {
            return _roomInformationRepositories.SearchRoom(keyword);
        }
        public void AddRoom(RoomInformation roomInformation)
        {
            _roomInformationRepositories.AddRoom(roomInformation);
        }
        public void DeleteRoom(int id)
        {
            _roomInformationRepositories.DeleteRoom(id);
        }
    }
}
