using HotelBooking.Application.Interfaces;
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

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}

