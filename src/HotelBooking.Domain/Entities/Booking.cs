using HotelBooking.Domain.Common;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities;

public class Booking : BaseEntity
{
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? SpecialRequests { get; set; }
    public int NumberOfGuests { get; set; }
    
    public int RoomId { get; set; }
    public string UserId { get; set; } = string.Empty;
    
    public virtual Room Room { get; set; } = null!;
}

