using HotelBooking.Application.DTOs;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Services;

public class BookingService(IUnitOfWork unitOfWork) : IBookingService
{
    public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
    {
        var bookings = await unitOfWork.Bookings.GetAllBookingsWithDetailsAsync();
        return bookings.Select(MapToDto);
    }

    public async Task<BookingDto?> GetBookingByIdAsync(int id)
    {
        var booking = await unitOfWork.Bookings.GetBookingWithDetailsAsync(id);
        return booking == null ? null : MapToDto(booking);
    }

    public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId)
    {
        var bookings = await unitOfWork.Bookings.GetBookingsByUserAsync(userId);
        return bookings.Select(MapToDto);
    }

    public async Task<BookingDto> CreateBookingAsync(string userId, CreateBookingDto dto)
    {
        var isAvailable = await unitOfWork.Rooms.IsRoomAvailableAsync(dto.RoomId, dto.CheckInDate, dto.CheckOutDate);
        if (!isAvailable)
        {
            throw new InvalidOperationException("Room is not available for the selected dates.");
        }

        var room = await unitOfWork.Rooms.GetByIdAsync(dto.RoomId);
        if (room == null)
        {
            throw new InvalidOperationException("Room not found.");
        }

        var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
        if (nights <= 0)
        {
            throw new InvalidOperationException("Check-out date must be after check-in date.");
        }

        var totalPrice = room.PricePerNight * nights;

        var booking = new Booking
        {
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            SpecialRequests = dto.SpecialRequests,
            NumberOfGuests = dto.NumberOfGuests,
            RoomId = dto.RoomId,
            UserId = userId
        };

        await unitOfWork.Bookings.AddAsync(booking);
        await unitOfWork.SaveChangesAsync();

        var createdBooking = await unitOfWork.Bookings.GetBookingWithDetailsAsync(booking.Id);
        return MapToDto(createdBooking!);
    }

    public async Task<BookingDto?> UpdateBookingStatusAsync(int id, UpdateBookingStatusDto dto)
    {
        var booking = await unitOfWork.Bookings.GetByIdAsync(id);
        if (booking == null) return null;

        booking.Status = dto.Status;
        booking.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Bookings.UpdateAsync(booking);
        await unitOfWork.SaveChangesAsync();

        var updatedBooking = await unitOfWork.Bookings.GetBookingWithDetailsAsync(booking.Id);
        return MapToDto(updatedBooking!);
    }

    public async Task<bool> CancelBookingAsync(int id, string userId)
    {
        var booking = await unitOfWork.Bookings.GetByIdAsync(id);
        if (booking == null)
        {
            throw new KeyNotFoundException("Бронювання не знайдено.");
        }

        if (booking.UserId != userId)
        {
            throw new UnauthorizedAccessException("Ви можете скасувати лише власні бронювання.");
        }

        if (booking.Status is BookingStatus.CheckedIn or BookingStatus.CheckedOut)
        {
            throw new InvalidOperationException("Неможливо скасувати бронювання, яке вже заселено або виселено.");
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Bookings.UpdateAsync(booking);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    private static BookingDto MapToDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status,
            SpecialRequests = booking.SpecialRequests,
            NumberOfGuests = booking.NumberOfGuests,
            RoomId = booking.RoomId,
            RoomNumber = booking.Room.RoomNumber,
            HotelName = booking.Room.Hotel.Name,
            UserId = booking.UserId,
            CreatedAt = booking.CreatedAt
        };
    }
}