namespace HotelBooking.Application.Models.Bookings;

public class CreateBookingModel
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string? SpecialRequests { get; set; }
    public int NumberOfGuests { get; set; }
    public int RoomId { get; set; }
}

