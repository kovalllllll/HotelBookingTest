using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Models.Rooms;

public class RoomModel
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public RoomType RoomType { get; set; }
    public string RoomTypeName => RoomType.ToString();
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
}

