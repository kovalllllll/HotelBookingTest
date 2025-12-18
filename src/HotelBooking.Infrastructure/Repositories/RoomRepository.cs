using HotelBooking.Application.Models.Rooms;
using HotelBooking.Application.Interfaces.Persistence;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class RoomRepository(ApplicationDbContext context) : Repository<Room>(context), IRoomRepository
{
    public async Task<IEnumerable<Room>> GetRoomsByHotelAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Hotel)
            .Where(r => r.HotelId == hotelId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Room>> SearchRoomsAsync(RoomFilter filter, CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(r => r.Hotel)
            .Include(r => r.Bookings)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.City))
        {
            query = query.Where(r => r.Hotel.City.Contains(filter.City, StringComparison.CurrentCultureIgnoreCase));
        }

        if (filter.Capacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= filter.Capacity.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(r => r.PricePerNight <= filter.MaxPrice.Value);
        }

        if (filter.RoomType.HasValue)
        {
            query = query.Where(r => r.RoomType == filter.RoomType.Value);
        }

        if (filter.CheckInDate.HasValue && filter.CheckOutDate.HasValue)
        {
            query = query.Where(r => !r.Bookings.Any(b =>
                b.Status != BookingStatus.Cancelled &&
                b.CheckInDate < filter.CheckOutDate.Value &&
                b.CheckOutDate > filter.CheckInDate.Value));
        }

        return await query.Where(r => r.IsAvailable).ToListAsync(cancellationToken);
    }

    public async Task<Room?> GetRoomWithHotelAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Bookings
            .Where(b => b.RoomId == roomId &&
                        b.Status != BookingStatus.Cancelled &&
                        b.CheckInDate < checkOut &&
                        b.CheckOutDate > checkIn);

        if (excludeBookingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookingId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }
}

