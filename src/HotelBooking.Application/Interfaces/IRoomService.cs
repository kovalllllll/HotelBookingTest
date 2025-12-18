using HotelBooking.Application.Models.Rooms;

namespace HotelBooking.Application.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<RoomModel>> GetAllRoomsAsync();
    Task<RoomModel?> GetRoomByIdAsync(int id);
    Task<IEnumerable<RoomModel>> GetRoomsByHotelAsync(int hotelId);
    Task<IEnumerable<RoomModel>> SearchRoomsAsync(RoomSearchModel searchModel);
    Task<RoomModel> CreateRoomAsync(CreateRoomModel model);
    Task<RoomModel?> UpdateRoomAsync(int id, UpdateRoomModel model);
    Task<bool> DeleteRoomAsync(int id);
}

