namespace HotelBooking.Application.Models.Statistics;

public class BookingStatisticsModel
{
    public int TotalBookings { get; set; }
    public int ConfirmedBookings { get; set; }
    public int CancelledBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<MonthlyBookingStats> MonthlyStats { get; set; } = [];
    public List<HotelBookingStats> HotelStats { get; set; } = [];
}

