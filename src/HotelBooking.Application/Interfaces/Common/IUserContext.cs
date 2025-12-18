namespace HotelBooking.Application.Interfaces.Common;

public interface IUserContext
{
    string? UserId { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}

