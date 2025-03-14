using AutoMapper;
using FluentValidation;
using ReservacionesHotel.Helpers;
using ReservacionesHotel.Interfaces.Repositories;
using ReservacionesHotel.Interfaces.Services;
using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;
using ReservacionesHotel.Validators;
using Serilog;

namespace ReservacionesHotel.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly IValidator<UserCreateDTO> _registerValidator;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository userRepository,
            JwtHelper jwtHelper,
            IValidator<UserCreateDTO> registerValidator,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _registerValidator = registerValidator;
            _mapper = mapper;
        }

        public async Task<(bool success, string token)> AuthenticateAsync(LoginDTO dto)
        {
            try
            {
                Log.Information("Init AuthenticateAsync");
                var user = await _userRepository.GetUserByEmailAsync(dto.Email);
                if (user == null)
                    return (false, string.Empty);

                if (!PasswordHasher.VerifyPassword(dto.PasswordHash, user.PasswordHash))
                    return (false, string.Empty);

                var token = await GenerateTokenAsync(user);
                return (true, token);
            }
            finally
            {
                Log.Information("End AuthenticateAsync");
            }
        }

        public async Task<Users> RegisterAsync(UserCreateDTO registerDto)
        {
            try
            {
                Log.Information("Init RegisterAsync");
                var validationResult = await _registerValidator.ValidateAsync(registerDto);
                if (!validationResult.IsValid)
                    throw new ValidationException(validationResult.Errors);

                registerDto.Password = PasswordHasher.HashPassword(registerDto.Password);
                registerDto.Role = string.IsNullOrEmpty(registerDto.Role) ? "User" : registerDto.Role;

                var user = _mapper.Map<Users>(registerDto);
                user.CreatedAt = DateTime.UtcNow;

                await _userRepository.AddAsync(user);
                return user;
            }
            finally
            {
                Log.Information("End RegisterAsync");
            }
        }

        public async Task<string> GenerateTokenAsync(Users user)
        {
            return _jwtHelper.GenerateToken(user.Id, user.Email, user.Role ?? "User");
        }
    }
}