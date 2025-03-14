using ReservacionesHotel.Helpers;
using ReservacionesHotel.Interfaces.Repositories;
using ReservacionesHotel.Interfaces.Services;
using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;
using AutoMapper;

namespace ReservacionesHotel.Services
{
    public class UserService(
        IUserRepository userRepository,
        IReservationRepository reservationRepository,
        IMapper mapper) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IReservationRepository _reservationRepository = reservationRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> CreateUserAsync(UserCreateDTO userCreateDto)
        {
            var user = _mapper.Map<Users>(userCreateDto);

            user.PasswordHash = PasswordHasher.HashPassword(userCreateDto.Password);
            user.CreatedAt = DateTime.UtcNow;

            await _userRepository.AddAsync(user);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> UpdateUserAsync(UserUpdateDTO userUpdateDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(userUpdateDto.Id) ?? throw new Exception("El usuario no existe.");

            _mapper.Map(userUpdateDto, existingUser);

            if (!string.IsNullOrEmpty(userUpdateDto.Password))
            {
                existingUser.PasswordHash = PasswordHasher.HashPassword(userUpdateDto.Password);
            }

            await _userRepository.UpdateAsync(existingUser);

            return _mapper.Map<UserDTO>(existingUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var userReservations = await _reservationRepository.GetUserReservationsAsync(id);
            if (userReservations.Any())
            {
                throw new Exception("No se puede eliminar un usuario con reservaciones activas.");
            }

            await _userRepository.DeleteAsync(user);
            return true;
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return false;

            return PasswordHasher.VerifyPassword(password, user.PasswordHash);
        }

        public async Task<IEnumerable<ReservationDTO>> GetUserReservationsAsync(int userId)
        {
            var reservations = await _reservationRepository.GetUserReservationsAsync(userId);
            return _mapper.Map<IEnumerable<ReservationDTO>>(reservations);
        }
    }
}