using HotelBooking.Application.DTOs;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Services;

public class RoomService(IUnitOfWork unitOfWork) : IRoomService
{
    public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
    {
        var rooms = await unitOfWork.Rooms.GetAllAsync();
        var result = new List<RoomDto>();
        foreach (var room in rooms)
        {
            var roomWithHotel = await unitOfWork.Rooms.GetRoomWithHotelAsync(room.Id);
            if (roomWithHotel != null)
                result.Add(MapToDto(roomWithHotel));
        }

        return result;
    }

    public async Task<RoomDto?> GetRoomByIdAsync(int id)
    {
        var room = await unitOfWork.Rooms.GetRoomWithHotelAsync(id);
        return room == null ? null : MapToDto(room);
    }

    public async Task<IEnumerable<RoomDto>> GetRoomsByHotelAsync(int hotelId)
    {
        var rooms = await unitOfWork.Rooms.GetRoomsByHotelAsync(hotelId);
        return rooms.Select(MapToDto);
    }

    public async Task<IEnumerable<RoomDto>> SearchRoomsAsync(RoomSearchDto searchDto)
    {
        var rooms = await unitOfWork.Rooms.SearchRoomsAsync(searchDto);
        return rooms.Select(MapToDto);
    }

    public async Task<RoomDto> CreateRoomAsync(CreateRoomDto dto)
    {
        var room = new Room
        {
            RoomNumber = dto.RoomNumber,
            RoomType = dto.RoomType,
            PricePerNight = dto.PricePerNight,
            Capacity = dto.Capacity,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            HotelId = dto.HotelId,
            IsAvailable = true
        };

        await unitOfWork.Rooms.AddAsync(room);
        await unitOfWork.SaveChangesAsync();

        var createdRoom = await unitOfWork.Rooms.GetRoomWithHotelAsync(room.Id);
        return MapToDto(createdRoom!);
    }

    public async Task<RoomDto?> UpdateRoomAsync(int id, UpdateRoomDto dto)
    {
        var room = await unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null) return null;

        room.RoomNumber = dto.RoomNumber;
        room.RoomType = dto.RoomType;
        room.PricePerNight = dto.PricePerNight;
        room.Capacity = dto.Capacity;
        room.Description = dto.Description;
        room.IsAvailable = dto.IsAvailable;
        room.ImageUrl = dto.ImageUrl;
        room.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Rooms.UpdateAsync(room);
        await unitOfWork.SaveChangesAsync();

        var updatedRoom = await unitOfWork.Rooms.GetRoomWithHotelAsync(room.Id);
        return MapToDto(updatedRoom!);
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null) return false;

        await unitOfWork.Rooms.DeleteAsync(room);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    private static RoomDto MapToDto(Room room)
    {
        return new RoomDto
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