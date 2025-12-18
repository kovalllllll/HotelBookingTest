using HotelBooking.Application.DTOs;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Services;

public class HotelService(IUnitOfWork unitOfWork) : IHotelService
{
    public async Task<IEnumerable<HotelDto>> GetAllHotelsAsync()
    {
        var hotels = await unitOfWork.Hotels.GetAllWithRoomsAsync();
        return hotels.Select(MapToDto);
    }

    public async Task<HotelDto?> GetHotelByIdAsync(int id)
    {
        var hotel = await unitOfWork.Hotels.GetHotelWithRoomsAsync(id);
        return hotel == null ? null : MapToDto(hotel);
    }

    public async Task<IEnumerable<HotelDto>> GetHotelsByCityAsync(string city)
    {
        var hotels = await unitOfWork.Hotels.GetHotelsByCityAsync(city);
        return hotels.Select(MapToDto);
    }

    public async Task<HotelDto> CreateHotelAsync(CreateHotelDto dto)
    {
        var hotel = new Hotel
        {
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            Description = dto.Description,
            StarRating = dto.StarRating,
            ImageUrl = dto.ImageUrl
        };

        await unitOfWork.Hotels.AddAsync(hotel);
        await unitOfWork.SaveChangesAsync();

        return MapToDto(hotel);
    }

    public async Task<HotelDto?> UpdateHotelAsync(int id, UpdateHotelDto dto)
    {
        var hotel = await unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null) return null;

        hotel.Name = dto.Name;
        hotel.Address = dto.Address;
        hotel.City = dto.City;
        hotel.Description = dto.Description;
        hotel.StarRating = dto.StarRating;
        hotel.ImageUrl = dto.ImageUrl;
        hotel.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Hotels.UpdateAsync(hotel);
        await unitOfWork.SaveChangesAsync();

        return MapToDto(hotel);
    }

    public async Task<bool> DeleteHotelAsync(int id)
    {
        var hotel = await unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null) return false;

        await unitOfWork.Hotels.DeleteAsync(hotel);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    private static HotelDto MapToDto(Hotel hotel)
    {
        return new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Address = hotel.Address,
            City = hotel.City,
            Description = hotel.Description,
            StarRating = hotel.StarRating,
            ImageUrl = hotel.ImageUrl,
            RoomsCount = hotel.Rooms.Count,
            MinPrice = hotel.Rooms.Count != 0 ? hotel.Rooms.Min(r => r.PricePerNight) : null
        };
    }
}

