using HotelBooking.Domain.Common;

namespace HotelBooking.Domain.Entities;

public class Hotel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public string? ImageUrl { get; set; }
    
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}

