using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservacionesHotel.Helpers;
using ReservacionesHotel.Interfaces.Services;
using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;

namespace ReservacionesHotel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public RoomController(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAllRooms()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            var roomDTOs = _mapper.Map<IEnumerable<RoomDTO>>(rooms);
            return Ok(roomDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDTO>> GetRoom(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            var roomDTO = _mapper.Map<RoomDTO>(room);
            return Ok(roomDTO);
        }

        [HttpGet("number/{roomNumber}")]
        public async Task<ActionResult<RoomDTO>> GetRoomByNumber(string roomNumber)
        {
            var room = await _roomService.GetRoomByNumberAsync(roomNumber);
            if (room == null)
            {
                return NotFound();
            }
            var roomDTO = _mapper.Map<RoomDTO>(room);
            return Ok(roomDTO);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<RoomDTO>>> GetAvailableRooms(
            [FromQuery] DateOnly checkIn, [FromQuery] DateOnly checkOut)
        {
            try
            {
                var rooms = await _roomService.GetAvailableRoomsAsync(checkIn, checkOut);
                var roomDTOs = _mapper.Map<IEnumerable<RoomDTO>>(rooms);
                return Ok(roomDTOs);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<RoomDTO>> CreateRoom(RoomCreateDTO roomCreateDTO)
        {
            try
            {
                var createdRoom = await _roomService.CreateRoomAsync(roomCreateDTO);
                var roomDTO = _mapper.Map<RoomDTO>(createdRoom);

                return CreatedAtAction(
                    nameof(GetRoom),
                    new { id = roomDTO.Id },
                    roomDTO
                );
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateRoom(int id, RoomUpdateDTO roomUpdateDTO)
        {
            try
            {
                var updatedRoom = await _roomService.UpdateRoomAsync(roomUpdateDTO);
                if (updatedRoom == null)
                    return NotFound();

                var roomDTO = _mapper.Map<RoomDTO>(updatedRoom);
                return Ok(roomDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                await _roomService.DeleteRoomAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
