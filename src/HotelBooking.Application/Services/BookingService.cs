using HotelBooking.Application.Models.Bookings;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Services;

public class BookingService(IUnitOfWork unitOfWork) : IBookingService
{
    public async Task<IEnumerable<BookingModel>> GetAllBookingsAsync()
    {
        var bookings = await unitOfWork.Bookings.GetAllBookingsWithDetailsAsync();
        return bookings.Select(MapToModel);
    }

    public async Task<BookingModel?> GetBookingByIdAsync(int id)
    {
        var booking = await unitOfWork.Bookings.GetBookingWithDetailsAsync(id);
        return booking == null ? null : MapToModel(booking);
    }

    public async Task<IEnumerable<BookingModel>> GetUserBookingsAsync(string userId)
    {
        var bookings = await unitOfWork.Bookings.GetBookingsByUserAsync(userId);
        return bookings.Select(MapToModel);
    }

    public async Task<BookingModel> CreateBookingAsync(string userId, CreateBookingModel model)
    {
        var isAvailable = await unitOfWork.Rooms.IsRoomAvailableAsync(model.RoomId, model.CheckInDate, model.CheckOutDate);
        if (!isAvailable)
        {
            throw new InvalidOperationException("Room is not available for the selected dates.");
        }

        var room = await unitOfWork.Rooms.GetByIdAsync(model.RoomId);
        if (room == null)
        {
            throw new InvalidOperationException("Room not found.");
        }

        var nights = (model.CheckOutDate - model.CheckInDate).Days;
        if (nights <= 0)
        {
            throw new InvalidOperationException("Check-out date must be after check-in date.");
        }

        var totalPrice = room.PricePerNight * nights;

        var booking = new Booking
        {
            CheckInDate = model.CheckInDate,
            CheckOutDate = model.CheckOutDate,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
            SpecialRequests = model.SpecialRequests,
            NumberOfGuests = model.NumberOfGuests,
            RoomId = model.RoomId,
            UserId = userId
        };

        await unitOfWork.Bookings.AddAsync(booking);
        await unitOfWork.SaveChangesAsync();

        var createdBooking = await unitOfWork.Bookings.GetBookingWithDetailsAsync(booking.Id);
        return MapToModel(createdBooking!);
    }

    public async Task<BookingModel?> UpdateBookingStatusAsync(int id, UpdateBookingStatusModel model)
    {
        var booking = await unitOfWork.Bookings.GetByIdAsync(id);
        if (booking == null) return null;

        booking.Status = model.Status;
        booking.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Bookings.UpdateAsync(booking);
        await unitOfWork.SaveChangesAsync();

        var updatedBooking = await unitOfWork.Bookings.GetBookingWithDetailsAsync(booking.Id);
        return MapToModel(updatedBooking!);
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

    private static BookingModel MapToModel(Booking booking)
    {
        return new BookingModel
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