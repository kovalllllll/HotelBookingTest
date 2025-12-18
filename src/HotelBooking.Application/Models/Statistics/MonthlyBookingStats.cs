namespace HotelBooking.Application.Models.Statistics;

public class MonthlyBookingStats
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int BookingsCount { get; set; }
    public decimal Revenue { get; set; }
}

