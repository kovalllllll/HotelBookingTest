namespace HotelBooking.Application.Models.Auth;

public class AuthResultModel
{
    public bool Succeeded { get; set; }
    public string? Error { get; set; }
    public UserModel? User { get; set; }
}

