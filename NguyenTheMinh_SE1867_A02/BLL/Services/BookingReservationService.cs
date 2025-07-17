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
                BookingReservationId = GetNextBookingReservationId(),
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

        public bool IsRoomAvailable(int roomId, DateOnly startDate, DateOnly endDate, int? excludeBookingId = null)
        {
            // Check if startDate is before endDate
            if (startDate >= endDate)
            {
                return false;
            }

            var conflictingBookings = _db.BookingDetails
                .Where(bd => bd.RoomId == roomId)
                .Where(bd => !excludeBookingId.HasValue || bd.BookingReservationId != excludeBookingId.Value)
                .Where(bd => 
                    (bd.StartDate <= startDate && bd.EndDate >= startDate) || 
                    (bd.StartDate <= endDate && bd.EndDate >= endDate) || 
                    (bd.StartDate >= startDate && bd.EndDate <= endDate) || 
                    (bd.StartDate <= startDate && bd.EndDate >= endDate) 
                );

            return !conflictingBookings.Any();
        }

        public void AddBookingWithDetails(BookingReservation booking, List<BookingDetail> bookingDetails)
        {
            foreach (var detail in bookingDetails)
            {
                if (detail.StartDate >= detail.EndDate)
                {
                    throw new InvalidOperationException($"Start date must be before end date for room {detail.RoomId}");
                }

                if (!IsRoomAvailable(detail.RoomId, detail.StartDate, detail.EndDate))
                {
                    var room = _db.RoomInformations.FirstOrDefault(r => r.RoomId == detail.RoomId);
                    var roomNumber = room?.RoomNumber ?? detail.RoomId.ToString();
                    throw new InvalidOperationException($"Room {roomNumber} is not available for the selected date range ({detail.StartDate:yyyy-MM-dd} to {detail.EndDate:yyyy-MM-dd})");
                }
            }

            booking.BookingReservationId = GetNextBookingReservationId();
            
            _db.BookingReservations.Add(booking);
            _db.SaveChanges();
            foreach (var detail in bookingDetails)
            {
                detail.BookingReservationId = booking.BookingReservationId;
                _db.BookingDetails.Add(detail);
            }
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

        public void UpdateBookingWithDetailsSmart(BookingReservation booking, List<BookingDetail> newDetails)
        {
            var existingBooking = _db.BookingReservations.Include(b => b.BookingDetails).FirstOrDefault(b => b.BookingReservationId == booking.BookingReservationId);
            if (existingBooking == null) return;

            foreach (var detail in newDetails)
            {
                if (detail.StartDate >= detail.EndDate)
                {
                    throw new InvalidOperationException($"Start date must be before end date for room {detail.RoomId}");
                }

                if (!IsRoomAvailable(detail.RoomId, detail.StartDate, detail.EndDate, booking.BookingReservationId))
                {
                    var room = _db.RoomInformations.FirstOrDefault(r => r.RoomId == detail.RoomId);
                    var roomNumber = room?.RoomNumber ?? detail.RoomId.ToString();
                    throw new InvalidOperationException($"Room {roomNumber} is not available for the selected date range ({detail.StartDate:yyyy-MM-dd} to {detail.EndDate:yyyy-MM-dd})");
                }
            }

            existingBooking.BookingDate = booking.BookingDate;
            existingBooking.CustomerId = booking.CustomerId;
            existingBooking.BookingStatus = booking.BookingStatus;
            existingBooking.TotalPrice = booking.TotalPrice;

            var toDelete = existingBooking.BookingDetails.Where(ed => !newDetails.Any(nd => nd.RoomId == ed.RoomId)).ToList();
            foreach (var del in toDelete)
            {
                _db.BookingDetails.Remove(del);
            }

            foreach (var nd in newDetails)
            {
                var existingDetail = existingBooking.BookingDetails.FirstOrDefault(ed => ed.RoomId == nd.RoomId);
                if (existingDetail != null)
                {
                    existingDetail.StartDate = nd.StartDate;
                    existingDetail.EndDate = nd.EndDate;
                    existingDetail.ActualPrice = nd.ActualPrice;
                }
                else
                {
                    nd.BookingReservationId = booking.BookingReservationId;
                    _db.BookingDetails.Add(nd);
                }
            }

            existingBooking.TotalPrice = existingBooking.BookingDetails.Sum(d => d.ActualPrice ?? 0);
            _db.SaveChanges();
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

        public int GetNextBookingReservationId()
        {
            var maxId = _db.BookingReservations.Max(b => (int?)b.BookingReservationId) ?? 0;
            return maxId + 1;
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