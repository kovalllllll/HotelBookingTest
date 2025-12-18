using HotelBooking.Application.DTOs;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBooking.Web.Pages;

public class IndexModel(IHotelService hotelService) : PageModel
{
    public IEnumerable<HotelDto> Hotels { get; set; } = [];
    
    [BindProperty(SupportsGet = true)]
    public string? City { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public DateTime? CheckIn { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public DateTime? CheckOut { get; set; }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrWhiteSpace(City))
        {
            Hotels = await hotelService.GetHotelsByCityAsync(City);
        }
        else
        {
            Hotels = await hotelService.GetAllHotelsAsync();
        }
    }
}

