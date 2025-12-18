using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Models.Rooms;

public class UpdateRoomModel
{
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }
}

