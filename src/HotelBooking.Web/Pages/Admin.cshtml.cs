using HotelBooking.Application.DTOs;
using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking.Web.Pages;

[Authorize(Roles = "Administrator")]
public class AdminModel(IBookingService bookingService, IStatisticsRepository statisticsRepository)
    : PageModel
{
    public BookingStatisticsDto Statistics { get; set; } = new();
    public IEnumerable<BookingDto> Bookings { get; set; } = Enumerable.Empty<BookingDto>();

    public async Task OnGetAsync()
    {
        Statistics = await statisticsRepository.GetBookingStatisticsAsync();
        Bookings = await bookingService.GetAllBookingsAsync();
    }

    public async Task<IActionResult> OnPostConfirmAsync(int bookingId)
    {
        await bookingService.UpdateBookingStatusAsync(bookingId, new UpdateBookingStatusDto 
        { 
            Status = BookingStatus.Confirmed 
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelAsync(int bookingId)
    {
        await bookingService.UpdateBookingStatusAsync(bookingId, new UpdateBookingStatusDto 
        { 
            Status = BookingStatus.Cancelled 
        });
        return RedirectToPage();
    }
}

