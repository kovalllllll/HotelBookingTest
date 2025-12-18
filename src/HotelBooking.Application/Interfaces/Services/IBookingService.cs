using HotelBooking.Application.Models.Bookings;

namespace HotelBooking.Application.Interfaces.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingModel>> GetAllBookingsAsync(CancellationToken cancellationToken = default);
    Task<BookingModel?> GetBookingByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingModel>> GetUserBookingsAsync(string userId, CancellationToken cancellationToken = default);
    Task<BookingModel> CreateBookingAsync(string userId, CreateBookingModel model, CancellationToken cancellationToken = default);
    Task<BookingModel?> UpdateBookingStatusAsync(int id, UpdateBookingStatusModel model, CancellationToken cancellationToken = default);
    Task<bool> CancelBookingAsync(int id, string userId, CancellationToken cancellationToken = default);
}

