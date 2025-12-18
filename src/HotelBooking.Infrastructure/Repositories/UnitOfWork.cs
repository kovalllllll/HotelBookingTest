using HotelBooking.Application.Interfaces.Persistence;
using HotelBooking.Infrastructure.Persistence;

namespace HotelBooking.Infrastructure.Repositories;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private IHotelRepository? _hotels;
    private IRoomRepository? _rooms;
    private IBookingRepository? _bookings;

    public IHotelRepository Hotels => _hotels ??= new HotelRepository(context);
    public IRoomRepository Rooms => _rooms ??= new RoomRepository(context);
    public IBookingRepository Bookings => _bookings ??= new BookingRepository(context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        context.Dispose();
    }
}