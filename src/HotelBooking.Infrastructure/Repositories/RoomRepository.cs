using HotelBooking.Application.Models.Rooms;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

public class RoomRepository(ApplicationDbContext context) : Repository<Room>(context), IRoomRepository
{
    public async Task<IEnumerable<Room>> GetRoomsByHotelAsync(int hotelId)
    {
        return await DbSet
            .Include(r => r.Hotel)
            .Where(r => r.HotelId == hotelId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Room>> SearchRoomsAsync(RoomSearchModel searchModel)
    {
        var query = DbSet
            .Include(r => r.Hotel)
            .Include(r => r.Bookings)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchModel.City))
        {
            query = query.Where(r => r.Hotel.City.Contains(searchModel.City, StringComparison.CurrentCultureIgnoreCase));
        }

        if (searchModel.Capacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= searchModel.Capacity.Value);
        }

        if (searchModel.MaxPrice.HasValue)
        {
            query = query.Where(r => r.PricePerNight <= searchModel.MaxPrice.Value);
        }

        if (searchModel.RoomType.HasValue)
        {
            query = query.Where(r => r.RoomType == searchModel.RoomType.Value);
        }

        if (searchModel.CheckInDate.HasValue && searchModel.CheckOutDate.HasValue)
        {
            query = query.Where(r => !r.Bookings.Any(b =>
                b.Status != BookingStatus.Cancelled &&
                b.CheckInDate < searchModel.CheckOutDate.Value &&
                b.CheckOutDate > searchModel.CheckInDate.Value));
        }

        return await query.Where(r => r.IsAvailable).ToListAsync();
    }

    public async Task<Room?> GetRoomWithHotelAsync(int id)
    {
        return await DbSet
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
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

        return !await query.AnyAsync();
    }
}

