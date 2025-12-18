using System.Security.Claims;
using HotelBooking.Application.DTOs;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking.Web.Pages.Hotels;

public class DetailsModel(
    IHotelService hotelService,
    IRoomService roomService,
    IBookingService bookingService)
    : PageModel
{
    public HotelDto? Hotel { get; set; }
    public IEnumerable<RoomDto> Rooms { get; set; } = [];
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Hotel = await hotelService.GetHotelByIdAsync(id);
        if (Hotel == null)
        {
            return Page();
        }

        Rooms = await roomService.GetRoomsByHotelAsync(id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, int roomId, DateTime checkIn, DateTime checkOut, int guests, string? specialRequests)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Login");
        }

        Hotel = await hotelService.GetHotelByIdAsync(id);
        Rooms = await roomService.GetRoomsByHotelAsync(id);

        if (checkOut <= checkIn)
        {
            ErrorMessage = "Дата виїзду має бути пізніше дати заїзду";
            return Page();
        }

        try
        {
            var booking = await bookingService.CreateBookingAsync(userId, new CreateBookingDto
            {
                RoomId = roomId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = guests,
                SpecialRequests = specialRequests
            });

            SuccessMessage = $"Бронювання успішно створено! Номер бронювання: #{booking.Id}. Загальна вартість: {booking.TotalPrice:N0} ₴";
            Rooms = await roomService.GetRoomsByHotelAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
        }

        return Page();
    }
}

