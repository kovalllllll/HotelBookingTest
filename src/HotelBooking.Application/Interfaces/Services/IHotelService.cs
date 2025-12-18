using HotelBooking.Application.Models.Hotels;

namespace HotelBooking.Application.Interfaces.Services;

public interface IHotelService
{
    Task<IEnumerable<HotelModel>> GetAllHotelsAsync(CancellationToken cancellationToken = default);
    Task<HotelModel?> GetHotelByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<HotelModel>> GetHotelsByCityAsync(string city, CancellationToken cancellationToken = default);
    Task<HotelModel> CreateHotelAsync(CreateHotelModel model, CancellationToken cancellationToken = default);
    Task<HotelModel?> UpdateHotelAsync(int id, UpdateHotelModel model, CancellationToken cancellationToken = default);
    Task<bool> DeleteHotelAsync(int id, CancellationToken cancellationToken = default);
}
