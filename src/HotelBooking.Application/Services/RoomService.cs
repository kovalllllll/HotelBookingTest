using HotelBooking.Application.Models.Rooms;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Services;

public class RoomService(IUnitOfWork unitOfWork) : IRoomService
{
    public async Task<IEnumerable<RoomModel>> GetAllRoomsAsync()
    {
        var rooms = await unitOfWork.Rooms.GetAllAsync();
        var result = new List<RoomModel>();
        foreach (var room in rooms)
        {
            var roomWithHotel = await unitOfWork.Rooms.GetRoomWithHotelAsync(room.Id);
            if (roomWithHotel != null)
                result.Add(MapToModel(roomWithHotel));
        }

        return result;
    }

    public async Task<RoomModel?> GetRoomByIdAsync(int id)
    {
        var room = await unitOfWork.Rooms.GetRoomWithHotelAsync(id);
        return room == null ? null : MapToModel(room);
    }

    public async Task<IEnumerable<RoomModel>> GetRoomsByHotelAsync(int hotelId)
    {
        var rooms = await unitOfWork.Rooms.GetRoomsByHotelAsync(hotelId);
        return rooms.Select(MapToModel);
    }

    public async Task<IEnumerable<RoomModel>> SearchRoomsAsync(RoomSearchModel searchModel)
    {
        var rooms = await unitOfWork.Rooms.SearchRoomsAsync(searchModel);
        return rooms.Select(MapToModel);
    }

    public async Task<RoomModel> CreateRoomAsync(CreateRoomModel model)
    {
        var room = new Room
        {
            RoomNumber = model.RoomNumber,
            RoomType = model.RoomType,
            PricePerNight = model.PricePerNight,
            Capacity = model.Capacity,
            Description = model.Description,
            ImageUrl = model.ImageUrl,
            HotelId = model.HotelId,
            IsAvailable = true
        };

        await unitOfWork.Rooms.AddAsync(room);
        await unitOfWork.SaveChangesAsync();

        var createdRoom = await unitOfWork.Rooms.GetRoomWithHotelAsync(room.Id);
        return MapToModel(createdRoom!);
    }

    public async Task<RoomModel?> UpdateRoomAsync(int id, UpdateRoomModel model)
    {
        var room = await unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null) return null;

        room.RoomNumber = model.RoomNumber;
        room.RoomType = model.RoomType;
        room.PricePerNight = model.PricePerNight;
        room.Capacity = model.Capacity;
        room.Description = model.Description;
        room.IsAvailable = model.IsAvailable;
        room.ImageUrl = model.ImageUrl;
        room.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Rooms.UpdateAsync(room);
        await unitOfWork.SaveChangesAsync();

        var updatedRoom = await unitOfWork.Rooms.GetRoomWithHotelAsync(room.Id);
        return MapToModel(updatedRoom!);
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null) return false;

        await unitOfWork.Rooms.DeleteAsync(room);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    private static RoomModel MapToModel(Room room)
    {
        return new RoomModel
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            RoomType = room.RoomType,
            PricePerNight = room.PricePerNight,
            Capacity = room.Capacity,
            Description = room.Description,
            IsAvailable = room.IsAvailable,
            ImageUrl = room.ImageUrl,
            HotelId = room.HotelId,
            HotelName = room.Hotel?.Name ?? string.Empty
        };
    }
}