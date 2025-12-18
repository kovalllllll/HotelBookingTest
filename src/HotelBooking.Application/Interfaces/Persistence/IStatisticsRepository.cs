using HotelBooking.Application.Models.Statistics;

namespace HotelBooking.Application.Interfaces.Persistence;

public interface IStatisticsRepository
{
    Task<BookingStatisticsModel> GetBookingStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
}

