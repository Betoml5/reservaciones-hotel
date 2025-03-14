using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservacionesHotel.Interfaces.Services;
using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;

namespace ReservacionesHotel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var (success, token) = await _authService.AuthenticateAsync(dto);

            if (!success)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDTO user)
        {
            try
            {
                var registeredUser = await _authService.RegisterAsync(user);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }


}