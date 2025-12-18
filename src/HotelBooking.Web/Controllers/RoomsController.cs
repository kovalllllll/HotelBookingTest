using HotelBooking.Application.Models.Rooms;
using HotelBooking.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController(IRoomService roomService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomModel>>> GetAll(CancellationToken cancellationToken)
    {
        var rooms = await roomService.GetAllRoomsAsync(cancellationToken);
        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoomModel>> GetById(int id, CancellationToken cancellationToken)
    {
        var room = await roomService.GetRoomByIdAsync(id, cancellationToken);
        if (room == null)
            return NotFound();
        return Ok(room);
    }


    [HttpGet("hotel/{hotelId:int}")]
    public async Task<ActionResult<IEnumerable<RoomModel>>> GetByHotel(int hotelId, CancellationToken cancellationToken)
    {
        var rooms = await roomService.GetRoomsByHotelAsync(hotelId, cancellationToken);
        return Ok(rooms);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<RoomModel>>> Search([FromQuery] RoomFilter filter, CancellationToken cancellationToken)
    {
        var rooms = await roomService.SearchRoomsAsync(filter, cancellationToken);
        return Ok(rooms);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<RoomModel>> Create([FromBody] CreateRoomModel model, CancellationToken cancellationToken)
    {
        var room = await roomService.CreateRoomAsync(model, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<RoomModel>> Update(int id, [FromBody] UpdateRoomModel model, CancellationToken cancellationToken)
    {
        var room = await roomService.UpdateRoomAsync(id, model, cancellationToken);
        if (room == null)
            return NotFound();
        return Ok(room);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await roomService.DeleteRoomAsync(id, cancellationToken);
        if (!result)
            return NotFound();
        return NoContent();
    }
}