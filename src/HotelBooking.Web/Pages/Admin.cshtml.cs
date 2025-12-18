using HotelBooking.Application.Models.Bookings;
using HotelBooking.Application.Models.Statistics;
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
    public BookingStatisticsModel Statistics { get; set; } = new();
    public IEnumerable<BookingModel> Bookings { get; set; } = Enumerable.Empty<BookingModel>();

    public async Task OnGetAsync()
    {
        Statistics = await statisticsRepository.GetBookingStatisticsAsync();
        Bookings = await bookingService.GetAllBookingsAsync();
    }

    public async Task<IActionResult> OnPostConfirmAsync(int bookingId)
    {
        await bookingService.UpdateBookingStatusAsync(bookingId, new UpdateBookingStatusModel 
        { 
            Status = BookingStatus.Confirmed 
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelAsync(int bookingId)
    {
        await bookingService.UpdateBookingStatusAsync(bookingId, new UpdateBookingStatusModel 
        { 
            Status = BookingStatus.Cancelled 
        });
        return RedirectToPage();
    }
}

