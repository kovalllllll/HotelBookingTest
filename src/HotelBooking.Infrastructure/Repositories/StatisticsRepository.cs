using System.Globalization;
using Dapper;
using HotelBooking.Application.Models.Statistics;
using HotelBooking.Application.Interfaces.Persistence;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class StatisticsRepository(ApplicationDbContext context) : IStatisticsRepository
{
    public async Task<BookingStatisticsModel> GetBookingStatisticsAsync(DateTime? startDate = null,
        DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();

        var start = startDate ?? DateTime.UtcNow.AddMonths(-12);
        var end = endDate ?? DateTime.UtcNow;

        const string overallStatsQuery = """

                                                     SELECT 
                                                         COUNT(*) as TotalBookings,
                                                         SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) as ConfirmedBookings,
                                                         SUM(CASE WHEN Status = 5 THEN 1 ELSE 0 END) as CancelledBookings,
                                                         COALESCE(SUM(CASE WHEN Status != 5 THEN TotalPrice ELSE 0 END), 0) as TotalRevenue
                                                     FROM Bookings
                                                     WHERE CreatedAt >= @StartDate AND CreatedAt <= @EndDate
                                         """;

        var overallStats =
            await connection.QueryFirstOrDefaultAsync<dynamic>(overallStatsQuery,
                new { StartDate = start, EndDate = end });

        const string monthlyStatsQuery = """

                                                     SELECT 
                                                         YEAR(CheckInDate) as Year,
                                                         MONTH(CheckInDate) as Month,
                                                         COUNT(*) as BookingsCount,
                                                         COALESCE(SUM(CASE WHEN Status != 5 THEN TotalPrice ELSE 0 END), 0) as Revenue
                                                     FROM Bookings
                                                     WHERE CheckInDate >= @StartDate AND CheckInDate <= @EndDate
                                                     GROUP BY YEAR(CheckInDate), MONTH(CheckInDate)
                                                     ORDER BY Year, Month
                                         """;

        var monthlyStatsResult =
            await connection.QueryAsync<dynamic>(monthlyStatsQuery, new { StartDate = start, EndDate = end });
        var monthlyStats = monthlyStatsResult
            .Select(x => new MonthlyBookingStats
            {
                Year = Convert.ToInt32(x.Year),
                Month = Convert.ToInt32(x.Month),
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(x.Month)),
                BookingsCount = Convert.ToInt32(x.BookingsCount),
                Revenue = Convert.ToDecimal(x.Revenue)
            }).ToList();

        const string hotelStatsQuery = """

                                                   SELECT 
                                                       h.Id as HotelId,
                                                       h.Name as HotelName,
                                                       COUNT(b.Id) as BookingsCount,
                                                       COALESCE(SUM(CASE WHEN b.Status != 5 THEN b.TotalPrice ELSE 0 END), 0) as Revenue
                                                   FROM Hotels h
                                                   LEFT JOIN Rooms r ON r.HotelId = h.Id
                                                   LEFT JOIN Bookings b ON b.RoomId = r.Id AND b.CheckInDate >= @StartDate AND b.CheckInDate <= @EndDate
                                                   GROUP BY h.Id, h.Name
                                                   ORDER BY BookingsCount DESC
                                       """;

        var hotelStatsResult =
            await connection.QueryAsync<dynamic>(hotelStatsQuery, new { StartDate = start, EndDate = end });
        var hotelStats = hotelStatsResult
            .Select(x => new HotelBookingStats
            {
                HotelId = Convert.ToInt32(x.HotelId),
                HotelName = (string)x.HotelName,
                BookingsCount = Convert.ToInt32(x.BookingsCount),
                Revenue = Convert.ToDecimal(x.Revenue),
                OccupancyRate = 0
            }).ToList();

        return new BookingStatisticsModel
        {
            TotalBookings = Convert.ToInt32(overallStats?.TotalBookings ?? 0),
            ConfirmedBookings = Convert.ToInt32(overallStats?.ConfirmedBookings ?? 0),
            CancelledBookings = Convert.ToInt32(overallStats?.CancelledBookings ?? 0),
            TotalRevenue = Convert.ToDecimal(overallStats?.TotalRevenue ?? 0m),
            MonthlyStats = monthlyStats,
            HotelStats = hotelStats
        };
    }
}