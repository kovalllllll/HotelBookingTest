namespace HotelBooking.Domain.Common;
public abstract class BaseEntity
{
    public DateTime? UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int Id { get; set; }
}



