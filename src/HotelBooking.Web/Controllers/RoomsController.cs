using HotelBooking.Application.DTOs;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController(IRoomService roomService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
    {
        var rooms = await roomService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        var room = await roomService.GetRoomByIdAsync(id);
        if (room == null)
            return NotFound();
        return Ok(room);
    }

    /// <summary>
    /// Gets rooms by hotel ID.
    /// Consider using GET /api/hotels/{hotelId}/rooms instead.
    /// </summary>
    [HttpGet("hotel/{hotelId:int}")]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetByHotel(int hotelId)
    {
        var rooms = await roomService.GetRoomsByHotelAsync(hotelId);
        return Ok(rooms);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<RoomDto>>> Search([FromQuery] RoomSearchDto searchDto)
    {
        var rooms = await roomService.SearchRoomsAsync(searchDto);
        return Ok(rooms);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto)
    {
        var room = await roomService.CreateRoomAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<RoomDto>> Update(int id, [FromBody] UpdateRoomDto dto)
    {
        var room = await roomService.UpdateRoomAsync(id, dto);
        if (room == null)
            return NotFound();
        return Ok(room);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await roomService.DeleteRoomAsync(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}

