using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Models.Rooms;

public class RoomSearchModel
{
    public string? City { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public int? Capacity { get; set; }
    public decimal? MaxPrice { get; set; }
    public RoomType? RoomType { get; set; }
}

