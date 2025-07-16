using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class BookingReservationService
    {
        private readonly FuminiHotelManagementContext _db;
        public BookingReservationService()
        {
            _db = new FuminiHotelManagementContext();
        }

        public List<BookingReservationDisplayModel> GetAllBookingReservationsWithDetails()
        {
            var query = from br in _db.BookingReservations
                        join c in _db.Customers on br.CustomerId equals c.CustomerId
                        from bd in br.BookingDetails
                        join r in _db.RoomInformations on bd.RoomId equals r.RoomId
                        select new BookingReservationDisplayModel
                        {
                            BookingReservationID = br.BookingReservationId,
                            BookingDate = br.BookingDate,
                            TotalPrice = br.TotalPrice,
                            CustomerID = c.CustomerId,
                            CustomerFullName = c.CustomerFullName,
                            Telephone = c.Telephone,
                            BookingStatus = br.BookingStatus,
                            RoomID = r.RoomId,
                            RoomNumber = r.RoomNumber,
                            StartDate = bd.StartDate,
                            EndDate = bd.EndDate,
                            ActualPrice = bd.ActualPrice
                        };
            return query.ToList();
        }

        public void AddBooking(BookingReservationDisplayModel model)
        {
            var booking = new BookingReservation
            {
                BookingReservationId = model.BookingReservationID,
                BookingDate = model.BookingDate,
                CustomerId = model.CustomerID,
                BookingStatus = model.BookingStatus,
            };
            _db.BookingReservations.Add(booking);
            _db.SaveChanges();
            var room = _db.RoomInformations.FirstOrDefault(r => r.RoomId == model.RoomID);
            decimal? actualPrice = null;
            if (room != null && room.RoomPricePerDay.HasValue)
            {
                int days = (model.EndDate.DayNumber - model.StartDate.DayNumber) + 1;
                actualPrice = room.RoomPricePerDay.Value * days;
            }
            var detail = new BookingDetail
            {
                BookingReservationId = booking.BookingReservationId,
                RoomId = model.RoomID,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ActualPrice = actualPrice
            };
            _db.BookingDetails.Add(detail);
            _db.SaveChanges();
            booking.TotalPrice = _db.BookingDetails
                .Where(d => d.BookingReservationId == booking.BookingReservationId)
                .Sum(d => d.ActualPrice ?? 0);
            _db.SaveChanges();
        }

        public void UpdateBooking(BookingReservationDisplayModel model)
        {
            var booking = _db.BookingReservations.Include(b => b.BookingDetails).FirstOrDefault(b => b.BookingReservationId == model.BookingReservationID);
            if (booking != null)
            {
                booking.BookingDate = model.BookingDate;
                booking.CustomerId = model.CustomerID;
                booking.BookingStatus = model.BookingStatus;
                var detail = booking.BookingDetails.FirstOrDefault(d => d.RoomId == model.RoomID);
                if (detail != null)
                {
                    detail.StartDate = model.StartDate;
                    detail.EndDate = model.EndDate;
                    var room = _db.RoomInformations.FirstOrDefault(r => r.RoomId == model.RoomID);
                    if (room != null && room.RoomPricePerDay.HasValue)
                    {
                        int days = (model.EndDate.DayNumber - model.StartDate.DayNumber) + 1;
                        detail.ActualPrice = room.RoomPricePerDay.Value * days;
                    }
                }
                _db.SaveChanges();
                booking.TotalPrice = booking.BookingDetails.Sum(d => d.ActualPrice ?? 0);
                _db.SaveChanges();
            }
        }

        public void DeleteBooking(int bookingReservationId, int roomId)
        {
            var detail = _db.BookingDetails.FirstOrDefault(d => d.BookingReservationId == bookingReservationId && d.RoomId == roomId);
            if (detail != null)
            {
                _db.BookingDetails.Remove(detail);
                _db.SaveChanges();
            }
            var booking = _db.BookingReservations.Include(b => b.BookingDetails).FirstOrDefault(b => b.BookingReservationId == bookingReservationId);
            if (booking != null && !booking.BookingDetails.Any())
            {
                _db.BookingReservations.Remove(booking);
                _db.SaveChanges();
            }
        }

        public List<BookingReservationDisplayModel> GetBookingReservationsByPeriod(DateOnly startDate, DateOnly endDate)
        {
            var query = from br in _db.BookingReservations
                        join c in _db.Customers on br.CustomerId equals c.CustomerId
                        from bd in br.BookingDetails
                        join r in _db.RoomInformations on bd.RoomId equals r.RoomId
                        where bd.StartDate >= startDate && bd.EndDate <= endDate
                        orderby bd.StartDate descending
                        select new BookingReservationDisplayModel
                        {
                            BookingReservationID = br.BookingReservationId,
                            BookingDate = br.BookingDate,
                            TotalPrice = br.TotalPrice,
                            CustomerID = c.CustomerId,
                            CustomerFullName = c.CustomerFullName,
                            Telephone = c.Telephone,
                            BookingStatus = br.BookingStatus,
                            RoomID = r.RoomId,
                            RoomNumber = r.RoomNumber,
                            StartDate = bd.StartDate,
                            EndDate = bd.EndDate,
                            ActualPrice = bd.ActualPrice
                        };
            return query.ToList();
        }
    }

    public class BookingReservationDisplayModel
    {
        public int BookingReservationID { get; set; }
        public System.DateOnly? BookingDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public int CustomerID { get; set; }
        public string? CustomerFullName { get; set; }
        public string? Telephone { get; set; }
        public byte? BookingStatus { get; set; }
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public System.DateOnly StartDate { get; set; }
        public System.DateOnly EndDate { get; set; }
        public decimal? ActualPrice { get; set; }
    }
} 