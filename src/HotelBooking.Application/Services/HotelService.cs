using HotelBooking.Application.Models.Hotels;
using HotelBooking.Application.Interfaces.Persistence;
using HotelBooking.Application.Interfaces.Services;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Services;

public class HotelService(IUnitOfWork unitOfWork) : IHotelService
{
    public async Task<IEnumerable<HotelModel>> GetAllHotelsAsync(CancellationToken cancellationToken = default)
    {
        var hotels = await unitOfWork.Hotels.GetAllWithRoomsAsync(cancellationToken);
        return hotels.Select(MapToModel);
    }

    public async Task<HotelModel?> GetHotelByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var hotel = await unitOfWork.Hotels.GetHotelWithRoomsAsync(id, cancellationToken);
        return hotel == null ? null : MapToModel(hotel);
    }

    public async Task<IEnumerable<HotelModel>> GetHotelsByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        var hotels = await unitOfWork.Hotels.GetHotelsByCityAsync(city, cancellationToken);
        return hotels.Select(MapToModel);
    }

    public async Task<HotelModel> CreateHotelAsync(CreateHotelModel model, CancellationToken cancellationToken = default)
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

        await unitOfWork.Hotels.AddAsync(hotel, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToModel(hotel);
    }

    public async Task<HotelModel?> UpdateHotelAsync(int id, UpdateHotelModel model, CancellationToken cancellationToken = default)
    {
        var hotel = await unitOfWork.Hotels.GetByIdAsync(id, cancellationToken);
        if (hotel == null) return null;

        hotel.Name = model.Name;
        hotel.Address = model.Address;
        hotel.City = model.City;
        hotel.Description = model.Description;
        hotel.StarRating = model.StarRating;
        hotel.ImageUrl = model.ImageUrl;
        hotel.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Hotels.UpdateAsync(hotel, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToModel(hotel);
    }

    public async Task<bool> DeleteHotelAsync(int id, CancellationToken cancellationToken = default)
    {
        var hotel = await unitOfWork.Hotels.GetByIdAsync(id, cancellationToken);
        if (hotel == null) return false;

        await unitOfWork.Hotels.DeleteAsync(hotel, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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

