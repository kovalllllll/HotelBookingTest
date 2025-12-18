namespace HotelBooking.Application.Models.Statistics;

public class HotelBookingStats
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public int BookingsCount { get; set; }
    public decimal Revenue { get; set; }
    public double OccupancyRate { get; set; }
}

