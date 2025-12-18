using HotelBooking.Application.DTOs;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController(IHotelService hotelService, IRoomService roomService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetAll()
    {
        var hotels = await hotelService.GetAllHotelsAsync();
        return Ok(hotels);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HotelDto>> GetById(int id)
    {
        var hotel = await hotelService.GetHotelByIdAsync(id);
        if (hotel == null)
            return NotFound();
        return Ok(hotel);
    }

    [HttpGet("{id:int}/rooms")]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms(int id)
    {
        var hotel = await hotelService.GetHotelByIdAsync(id);
        if (hotel == null)
            return NotFound();
        
        var rooms = await roomService.GetRoomsByHotelAsync(id);
        return Ok(rooms);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<HotelDto>>> SearchByCity([FromQuery] string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest("City is required");
        
        var hotels = await hotelService.GetHotelsByCityAsync(city);
        return Ok(hotels);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<HotelDto>> Create([FromBody] CreateHotelDto dto)
    {
        var hotel = await hotelService.CreateHotelAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<HotelDto>> Update(int id, [FromBody] UpdateHotelDto dto)
    {
        var hotel = await hotelService.UpdateHotelAsync(id, dto);
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

