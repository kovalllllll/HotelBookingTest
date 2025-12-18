using HotelBooking.Application.Interfaces.Persistence;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class HotelRepository(ApplicationDbContext context) : Repository<Hotel>(context), IHotelRepository
{
    public async Task<IEnumerable<Hotel>> GetHotelsByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(h => h.Rooms)
            .Where(h => h.City.Contains(city, StringComparison.CurrentCultureIgnoreCase))
            .ToListAsync(cancellationToken);
    }

    public async Task<Hotel?> GetHotelWithRoomsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Hotel>> GetAllWithRoomsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(h => h.Rooms)
            .ToListAsync(cancellationToken);
    }
}

