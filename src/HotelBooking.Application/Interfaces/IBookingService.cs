using HotelBooking.Application.Models.Bookings;

namespace HotelBooking.Application.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingModel>> GetAllBookingsAsync();
    Task<BookingModel?> GetBookingByIdAsync(int id);
    Task<IEnumerable<BookingModel>> GetUserBookingsAsync(string userId);
    Task<BookingModel> CreateBookingAsync(string userId, CreateBookingModel model);
    Task<BookingModel?> UpdateBookingStatusAsync(int id, UpdateBookingStatusModel model);
    Task<bool> CancelBookingAsync(int id, string userId);
}

