using AutoMapper;
using ReservacionesHotel.Interfaces.Repositories;
using ReservacionesHotel.Interfaces.Services;
using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;
using Serilog;

namespace ReservacionesHotel.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public ReservationService(
            IReservationRepository reservationRepository,
            IRoomRepository roomRepository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _reservationRepository = reservationRepository;
            _roomRepository = roomRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync()
        {
            try
            {
                Log.Information("Init GetAllReservationsAsync");
                var reservations = await _reservationRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<ReservationDTO>>(reservations);
            }
            finally
            {
                Log.Information("End GetAllReservationsAsync");
            }
        }

        public async Task<ReservationDetailsDTO> GetReservationByIdAsync(int id)
        {
            try
            {
                var reservation = await _reservationRepository.GetByIdAsync(id);
                return _mapper.Map<ReservationDetailsDTO>(reservation);
            }
            finally
            {
                Log.Information("End GetReservationByIdAsync");
            }
        }

        public async Task<IEnumerable<ReservationDTO>> GetUserReservationsAsync(int userId)
        {
            try
            {
                var reservations = await _reservationRepository.GetUserReservationsAsync(userId);
                return _mapper.Map<IEnumerable<ReservationDTO>>(reservations);
            }
            finally
            {
                Log.Information("End GetUserReservationsAsync");
            }
        }

        public async Task<IEnumerable<ReservationDTO>> GetRoomReservationsAsync(int roomId)
        {
            try
            {
                var reservations = await _reservationRepository.GetRoomReservationsAsync(roomId);
                return _mapper.Map<IEnumerable<ReservationDTO>>(reservations);
            }
            finally
            {
                Log.Information("End GetRoomReservationsAsync");
            }
        }

        public async Task<ReservationDTO> CreateReservationAsync(ReservationCreateDTO reservationCreateDTO)
        {
            try
            {
                // Verificar disponibilidad
                if (!await IsRoomAvailableForDatesAsync(reservationCreateDTO.RoomId, reservationCreateDTO.CheckInDate, reservationCreateDTO.CheckOutDate))
                    throw new Exception("La habitación no está disponible para las fechas seleccionadas.");

                var user = await _currentUserService.GetCurrentUserAsync() ?? throw new Exception("Usuario no autenticado");
                var totalPrice = await CalculateTotalPriceAsync(reservationCreateDTO.RoomId, reservationCreateDTO.CheckInDate, reservationCreateDTO.CheckOutDate);

                var reservation = _mapper.Map<Reservations>(reservationCreateDTO);


                reservation.UserId = user.Id;
                reservation.TotalPrice = totalPrice;
                reservation.CreatedAt = DateTime.UtcNow;
                reservation.Status = ReservationsStatus.BOOKED;

                await _reservationRepository.AddAsync(reservation);
                return _mapper.Map<ReservationDTO>(reservation);
            }
            finally
            {
                Log.Information("End CreateReservationAsync");
            }
        }

        public async Task<ReservationDTO> UpdateReservationAsync(ReservationUpdateDTO reservationUpdateDTO, int id)
        {
            try
            {
                var existingReservation = await _reservationRepository.GetByIdAsync(id) ?? throw new Exception("La reservación no existe.");

                if (reservationUpdateDTO.Id != existingReservation.RoomId ||
                    reservationUpdateDTO.CheckInDate != existingReservation.CheckInDate ||
                    reservationUpdateDTO.CheckOutDate != existingReservation.CheckOutDate)
                {
                    if (!await IsRoomAvailableForDatesAsync(reservationUpdateDTO.RoomId, reservationUpdateDTO.CheckInDate, reservationUpdateDTO.CheckOutDate))
                        throw new Exception("La habitación no está disponible para las fechas seleccionadas.");
                }

                var totalPrice = await CalculateTotalPriceAsync(reservationUpdateDTO.RoomId, reservationUpdateDTO.CheckInDate, reservationUpdateDTO.CheckOutDate);
                var updatedReservation = _mapper.Map(reservationUpdateDTO, existingReservation);
                updatedReservation.TotalPrice = totalPrice;

                await _reservationRepository.UpdateAsync(updatedReservation);
                return _mapper.Map<ReservationDTO>(updatedReservation);
            }
            finally
            {
                Log.Information("End UpdateReservationAsync");
            }
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            try
            {
                var reservation = await _reservationRepository.GetByIdAsync(id);
                if (reservation != null)
                {
                    await _reservationRepository.DeleteAsync(reservation);
                    return true;
                }
                return false;
            }
            finally
            {
                Log.Information("End DeleteReservationAsync");
            }
        }

        public async Task<bool> IsRoomAvailableForDatesAsync(int roomId, DateOnly checkIn, DateOnly checkOut)
        {
            return await _reservationRepository.IsRoomAvailableForDatesAsync(roomId, checkIn, checkOut);
        }

        public async Task<decimal> CalculateTotalPriceAsync(int roomId, DateOnly checkIn, DateOnly checkOut)
        {
            try
            {
                Log.Information($"Init {nameof(CalculateTotalPriceAsync)}");
                var room = await _roomRepository.GetByIdAsync(roomId) ?? throw new Exception("La habitación no existe.");

                var days = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;

                return room.PricePerNight * days;
            }
            finally
            {
                Log.Information($"End {nameof(CalculateTotalPriceAsync)}");
            }
        }
    }
}