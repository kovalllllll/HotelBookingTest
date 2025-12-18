namespace HotelBooking.Application.Models.Hotels;

public class CreateHotelModel
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public string? ImageUrl { get; set; }
}

