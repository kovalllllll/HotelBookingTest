using HotelBooking.Application.Models.Rooms;

namespace HotelBooking.Application.Interfaces.Services;

public interface IRoomService
{
    Task<IEnumerable<RoomModel>> GetAllRoomsAsync(CancellationToken cancellationToken = default);
    Task<RoomModel?> GetRoomByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoomModel>> GetRoomsByHotelAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoomModel>> SearchRoomsAsync(RoomFilter filter, CancellationToken cancellationToken = default);
    Task<RoomModel> CreateRoomAsync(CreateRoomModel model, CancellationToken cancellationToken = default);
    Task<RoomModel?> UpdateRoomAsync(int id, UpdateRoomModel model, CancellationToken cancellationToken = default);
    Task<bool> DeleteRoomAsync(int id, CancellationToken cancellationToken = default);
}

