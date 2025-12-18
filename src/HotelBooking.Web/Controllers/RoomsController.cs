using HotelBooking.Application.Models.Rooms;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController(IRoomService roomService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomModel>>> GetAll()
    {
        var rooms = await roomService.GetAllRoomsAsync();
        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoomModel>> GetById(int id)
    {
        var room = await roomService.GetRoomByIdAsync(id);
        if (room == null)
            return NotFound();
        return Ok(room);
    }


    [HttpGet("hotel/{hotelId:int}")]
    public async Task<ActionResult<IEnumerable<RoomModel>>> GetByHotel(int hotelId)
    {
        var rooms = await roomService.GetRoomsByHotelAsync(hotelId);
        return Ok(rooms);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<RoomModel>>> Search([FromQuery] RoomSearchModel searchModel)
    {
        var rooms = await roomService.SearchRoomsAsync(searchModel);
        return Ok(rooms);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<RoomModel>> Create([FromBody] CreateRoomModel model)
    {
        var room = await roomService.CreateRoomAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<RoomModel>> Update(int id, [FromBody] UpdateRoomModel model)
    {
        var room = await roomService.UpdateRoomAsync(id, model);
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