using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Models.Bookings;

public class UpdateBookingStatusModel
{
    public BookingStatus Status { get; set; }
}

