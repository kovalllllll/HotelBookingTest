using HotelBooking.Application.Models.Hotels;
using HotelBooking.Application.Models.Rooms;
using HotelBooking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController(IHotelService hotelService, IRoomService roomService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelModel>>> GetAll(CancellationToken cancellationToken)
    {
        var hotels = await hotelService.GetAllHotelsAsync(cancellationToken);
        return Ok(hotels);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HotelModel>> GetById(int id, CancellationToken cancellationToken)
    {
        var hotel = await hotelService.GetHotelByIdAsync(id, cancellationToken);
        if (hotel == null)
            return NotFound();
        return Ok(hotel);
    }

    [HttpGet("{id:int}/rooms")]
    public async Task<ActionResult<IEnumerable<RoomModel>>> GetRooms(int id, CancellationToken cancellationToken)
    {
        var hotel = await hotelService.GetHotelByIdAsync(id, cancellationToken);
        if (hotel == null)
            return NotFound();

        var rooms = await roomService.GetRoomsByHotelAsync(id, cancellationToken);
        return Ok(rooms);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<HotelModel>>> SearchByCity([FromQuery] string city, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("City is required");

        var hotels = await hotelService.GetHotelsByCityAsync(city, cancellationToken);
        return Ok(hotels);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<HotelModel>> Create([FromBody] CreateHotelModel model, CancellationToken cancellationToken)
    {
        var hotel = await hotelService.CreateHotelAsync(model, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<HotelModel>> Update(int id, [FromBody] UpdateHotelModel model, CancellationToken cancellationToken)
    {
        var hotel = await hotelService.UpdateHotelAsync(id, model, cancellationToken);
        if (hotel == null)
            return NotFound();
        return Ok(hotel);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await hotelService.DeleteHotelAsync(id, cancellationToken);
        if (!result)
            return NotFound();
        return NoContent();
    }
}