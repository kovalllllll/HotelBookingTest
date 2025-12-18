using HotelBooking.Application.Models.Hotels;

namespace HotelBooking.Application.Interfaces;

public interface IHotelService
{
    Task<IEnumerable<HotelModel>> GetAllHotelsAsync();
    Task<HotelModel?> GetHotelByIdAsync(int id);
    Task<IEnumerable<HotelModel>> GetHotelsByCityAsync(string city);
    Task<HotelModel> CreateHotelAsync(CreateHotelModel model);
    Task<HotelModel?> UpdateHotelAsync(int id, UpdateHotelModel model);
    Task<bool> DeleteHotelAsync(int id);
}
