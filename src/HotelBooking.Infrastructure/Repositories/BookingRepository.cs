using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class BookingRepository(ApplicationDbContext context) : Repository<Booking>(context), IBookingRepository
{
    public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(string userId)
    {
        return await DbSet
            .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetAllBookingsWithDetailsAsync()
    {
        return await DbSet
            .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Booking?> GetBookingWithDetailsAsync(int id)
    {
        return await DbSet
            .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetBookingsByRoomAsync(int roomId)
    {
        return await DbSet
            .Where(b => b.RoomId == roomId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetBookingsInDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await DbSet
            .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
            .Where(b => b.CheckInDate >= startDate && b.CheckOutDate <= endDate)
            .ToListAsync();
    }
}

