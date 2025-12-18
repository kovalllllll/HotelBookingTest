namespace HotelBooking.Application.Interfaces;

public interface IUserContext
{
    string? UserId { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}

