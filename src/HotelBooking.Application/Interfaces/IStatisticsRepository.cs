using HotelBooking.Application.Models.Statistics;

namespace HotelBooking.Application.Interfaces;

public interface IStatisticsRepository
{
    Task<BookingStatisticsModel> GetBookingStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
}

