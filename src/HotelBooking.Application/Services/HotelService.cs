using HotelBooking.Application.Models.Hotels;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Services;

public class HotelService(IUnitOfWork unitOfWork) : IHotelService
{
    public async Task<IEnumerable<HotelModel>> GetAllHotelsAsync()
    {
        var hotels = await unitOfWork.Hotels.GetAllWithRoomsAsync();
        return hotels.Select(MapToModel);
    }

    public async Task<HotelModel?> GetHotelByIdAsync(int id)
    {
        var hotel = await unitOfWork.Hotels.GetHotelWithRoomsAsync(id);
        return hotel == null ? null : MapToModel(hotel);
    }

    public async Task<IEnumerable<HotelModel>> GetHotelsByCityAsync(string city)
    {
        var hotels = await unitOfWork.Hotels.GetHotelsByCityAsync(city);
        return hotels.Select(MapToModel);
    }

    public async Task<HotelModel> CreateHotelAsync(CreateHotelModel model)
    {
        var hotel = new Hotel
        {
            Name = model.Name,
            Address = model.Address,
            City = model.City,
            Description = model.Description,
            StarRating = model.StarRating,
            ImageUrl = model.ImageUrl
        };

        await unitOfWork.Hotels.AddAsync(hotel);
        await unitOfWork.SaveChangesAsync();

        return MapToModel(hotel);
    }

    public async Task<HotelModel?> UpdateHotelAsync(int id, UpdateHotelModel model)
    {
        var hotel = await unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null) return null;

        hotel.Name = model.Name;
        hotel.Address = model.Address;
        hotel.City = model.City;
        hotel.Description = model.Description;
        hotel.StarRating = model.StarRating;
        hotel.ImageUrl = model.ImageUrl;
        hotel.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Hotels.UpdateAsync(hotel);
        await unitOfWork.SaveChangesAsync();

        return MapToModel(hotel);
    }

    public async Task<bool> DeleteHotelAsync(int id)
    {
        var hotel = await unitOfWork.Hotels.GetByIdAsync(id);
        if (hotel == null) return false;

        await unitOfWork.Hotels.DeleteAsync(hotel);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    private static HotelModel MapToModel(Hotel hotel)
    {
        return new HotelModel
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

