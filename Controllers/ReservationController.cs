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
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ICurrentUserService _currentUserService;

        public ReservationController(
            IReservationService reservationService,
            ICurrentUserService currentUserService)
        {
            _reservationService = reservationService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetAllReservations()
        {
            var reservations = await _reservationService.GetAllReservationsAsync();
            return Ok(reservations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            // Solo permitir ver la reservación si es el usuario propietario o admin
            // if (!_currentUserService.IsAdmin && reservation.UserId != _currentUserService.UserId)
            // {
            //     return Forbid();
            // }

            return Ok(reservation);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetUserReservations(int userId)
        {
            var reservations = await _reservationService.GetUserReservationsAsync(userId);
            return Ok(reservations);
        }

        [HttpGet("my-reservations")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetMyReservations()
        {
            if (!_currentUserService.UserId.HasValue)
                return Unauthorized();

            var reservations = await _reservationService.GetUserReservationsAsync(_currentUserService.UserId.Value);
            return Ok(reservations);
        }

        [HttpGet("room/{roomId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetRoomReservations(int roomId)
        {
            var reservations = await _reservationService.GetRoomReservationsAsync(roomId);
            return Ok(reservations);
        }

        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> CreateReservation(ReservationCreateDTO reservationCreateDto)
        {
            try
            {
                // Si no es admin, el UserId será establecido por el servicio
                var createdReservation = await _reservationService.CreateReservationAsync(reservationCreateDto);
                return CreatedAtAction(
                    nameof(GetReservation),
                    new { id = createdReservation.Id },
                    createdReservation
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, ReservationUpdateDTO reservationUpdateDto)
        {
            var existingReservation = await _reservationService.GetReservationByIdAsync(id);
            if (existingReservation == null)
            {
                return NotFound();
            }

            // // Solo permitir actualizar si es el usuario propietario o admin
            // if (!_currentUserService.IsAdmin && existingReservation.UserId != _currentUserService.UserId)
            // {
            //     return Forbid();
            // }

            try
            {
                // ID para la actualización debe venir del parámetro de la ruta, no del DTO
                var updatedReservation = await _reservationService.UpdateReservationAsync(reservationUpdateDto, id);
                return Ok(updatedReservation);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            // Solo permitir eliminar si es el usuario propietario o admin
            // if (!_currentUserService.IsAdmin && reservation.UserId != _currentUserService.UserId)
            // {
            //     return Forbid();
            // }

            var result = await _reservationService.DeleteReservationAsync(id);
            if (!result)
            {
                return BadRequest(new { message = "No se pudo eliminar la reservación" });
            }

            return NoContent();
        }

        [HttpGet("price-estimate")]
        public async Task<ActionResult<decimal>> GetPriceEstimate(
            [FromQuery] int roomId,
            [FromQuery] DateOnly checkIn,
            [FromQuery] DateOnly checkOut)
        {
            try
            {
                var price = await _reservationService.CalculateTotalPriceAsync(roomId, checkIn, checkOut);
                return Ok(new { totalPrice = price });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("availability")]
        public async Task<ActionResult<bool>> CheckAvailability(
            [FromQuery] int roomId,
            [FromQuery] DateOnly checkIn,
            [FromQuery] DateOnly checkOut)
        {
            try
            {
                var isAvailable = await _reservationService.IsRoomAvailableForDatesAsync(roomId, checkIn, checkOut);
                return Ok(new { available = isAvailable });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
