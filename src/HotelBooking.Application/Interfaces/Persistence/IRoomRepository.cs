using HotelBooking.Application.Models.Rooms;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Interfaces.Persistence;

public interface IRoomRepository : IRepository<Room>
{
    Task<IEnumerable<Room>> GetRoomsByHotelAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Room>> SearchRoomsAsync(RoomFilter filter, CancellationToken cancellationToken = default);
    Task<Room?> GetRoomWithHotelAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null, CancellationToken cancellationToken = default);
}

