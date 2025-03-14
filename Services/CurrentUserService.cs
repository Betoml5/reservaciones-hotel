using Microsoft.AspNetCore.Http;
using ReservacionesHotel.Helpers;
using ReservacionesHotel.Interfaces.Repositories;
using ReservacionesHotel.Interfaces.Services;
using ReservacionesHotel.Models.Entities;
using System.Security.Claims;

namespace ReservacionesHotel.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private Users? _currentUser;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public int? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return userIdClaim != null ? int.Parse(userIdClaim) : null;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

        public string? Role => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

        public bool IsAdmin => Role == Roles.Admin;

        public async Task<Users?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
                return _currentUser;

            if (!IsAuthenticated || UserId == null)
                return null;

            _currentUser = await _userRepository.GetByIdAsync(UserId.Value);
            return _currentUser;
        }
    }
}