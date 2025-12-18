using HotelBooking.Application.DTOs;
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

    public async Task<IEnumerable<Room>> SearchRoomsAsync(RoomSearchDto searchDto)
    {
        var query = DbSet
            .Include(r => r.Hotel)
            .Include(r => r.Bookings)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchDto.City))
        {
            query = query.Where(r => r.Hotel.City.Contains(searchDto.City, StringComparison.CurrentCultureIgnoreCase));
        }

        if (searchDto.Capacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= searchDto.Capacity.Value);
        }

        if (searchDto.MaxPrice.HasValue)
        {
            query = query.Where(r => r.PricePerNight <= searchDto.MaxPrice.Value);
        }

        if (searchDto.RoomType.HasValue)
        {
            query = query.Where(r => r.RoomType == searchDto.RoomType.Value);
        }

        if (searchDto.CheckInDate.HasValue && searchDto.CheckOutDate.HasValue)
        {
            query = query.Where(r => !r.Bookings.Any(b =>
                b.Status != BookingStatus.Cancelled &&
                b.CheckInDate < searchDto.CheckOutDate.Value &&
                b.CheckOutDate > searchDto.CheckInDate.Value));
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

