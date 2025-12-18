using HotelBooking.Application.Interfaces.Persistence;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class BookingRepository(ApplicationDbContext context) : Repository<Booking>(context), IBookingRepository
{
    public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(string userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetAllBookingsWithDetailsAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Booking?> GetBookingWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByRoomAsync(int roomId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(b => b.RoomId == roomId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetBookingsInDateRangeAsync(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .Where(b => b.CheckInDate >= startDate && b.CheckOutDate <= endDate)
            .ToListAsync(cancellationToken);
    }
}