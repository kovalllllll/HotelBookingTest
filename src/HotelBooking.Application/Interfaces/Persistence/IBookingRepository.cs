using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Interfaces.Persistence;

public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetBookingsByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetAllBookingsWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<Booking?> GetBookingWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetBookingsByRoomAsync(int roomId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetBookingsInDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}