namespace HotelBooking.Application.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    IHotelRepository Hotels { get; }
    IRoomRepository Rooms { get; }
    IBookingRepository Bookings { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

