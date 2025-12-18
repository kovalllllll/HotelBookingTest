using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class HotelRepository(ApplicationDbContext context) : Repository<Hotel>(context), IHotelRepository
{
    public async Task<IEnumerable<Hotel>> GetHotelsByCityAsync(string city)
    {
        return await DbSet
            .Include(h => h.Rooms)
            .Where(h => h.City.Contains(city, StringComparison.CurrentCultureIgnoreCase))
            .ToListAsync();
    }

    public async Task<Hotel?> GetHotelWithRoomsAsync(int id)
    {
        return await DbSet
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<IEnumerable<Hotel>> GetAllWithRoomsAsync()
    {
        return await DbSet
            .Include(h => h.Rooms)
            .ToListAsync();
    }
}

