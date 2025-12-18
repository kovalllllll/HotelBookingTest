using HotelBooking.Application.Models.Rooms;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Interfaces;

public interface IRoomRepository : IRepository<Room>
{
    Task<IEnumerable<Room>> GetRoomsByHotelAsync(int hotelId);
    Task<IEnumerable<Room>> SearchRoomsAsync(RoomSearchModel searchModel);
    Task<Room?> GetRoomWithHotelAsync(int id);
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null);
}

