using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Models.Rooms;

public class CreateRoomModel
{
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int HotelId { get; set; }
}

