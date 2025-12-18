using HotelBooking.Application.Models.Hotels;
using HotelBooking.Application.Models.Rooms;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController(IHotelService hotelService, IRoomService roomService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelModel>>> GetAll()
    {
        var hotels = await hotelService.GetAllHotelsAsync();
        return Ok(hotels);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HotelModel>> GetById(int id)
    {
        var hotel = await hotelService.GetHotelByIdAsync(id);
        if (hotel == null)
            return NotFound();
        return Ok(hotel);
    }

    [HttpGet("{id:int}/rooms")]
    public async Task<ActionResult<IEnumerable<RoomModel>>> GetRooms(int id)
    {
        var hotel = await hotelService.GetHotelByIdAsync(id);
        if (hotel == null)
            return NotFound();

        var rooms = await roomService.GetRoomsByHotelAsync(id);
        return Ok(rooms);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<HotelModel>>> SearchByCity([FromQuery] string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("City is required");

        var hotels = await hotelService.GetHotelsByCityAsync(city);
        return Ok(hotels);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<HotelModel>> Create([FromBody] CreateHotelModel model)
    {
        var hotel = await hotelService.CreateHotelAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<HotelModel>> Update(int id, [FromBody] UpdateHotelModel model)
    {
        var hotel = await hotelService.UpdateHotelAsync(id, model);
        if (hotel == null)
            return NotFound();
        return Ok(hotel);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await hotelService.DeleteHotelAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}