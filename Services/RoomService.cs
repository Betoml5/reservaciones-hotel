using AutoMapper;
using ReservacionesHotel.Interfaces.Repositories;
using ReservacionesHotel.Interfaces.Services;
using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;

namespace ReservacionesHotel.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;

        public RoomService(IRoomRepository roomRepository, IReservationRepository reservationRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoomDTO>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoomDTO>>(rooms);
        }

        public async Task<RoomDTO> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            return _mapper.Map<RoomDTO>(room);
        }

        public async Task<RoomDTO> GetRoomByNumberAsync(string roomNumber)
        {
            var room = await _roomRepository.GetRoomByNumberAsync(roomNumber);
            return _mapper.Map<RoomDTO>(room);
        }

        public async Task<IEnumerable<RoomDTO>> GetAvailableRoomsAsync(DateOnly checkIn, DateOnly checkOut)
        {
            var rooms = await _roomRepository.GetAvailableRoomsAsync(checkIn, checkOut);
            return _mapper.Map<IEnumerable<RoomDTO>>(rooms);
        }

        public async Task<RoomDTO> CreateRoomAsync(RoomCreateDTO roomCreateDTO)
        {
            var existingRoom = await _roomRepository.GetRoomByNumberAsync(roomCreateDTO.RoomNumber);
            if (existingRoom != null)
                throw new Exception($"Ya existe una habitación con el número {roomCreateDTO.RoomNumber}");

            // Mapear DTO a entidad
            var entity = _mapper.Map<Rooms>(roomCreateDTO);

            entity.Status = roomCreateDTO.IsAvailable ? RoomsStatus.AVAILABLE : RoomsStatus.OCCUPIED;

            await _roomRepository.AddAsync(entity);

            return _mapper.Map<RoomDTO>(entity);
        }

        public async Task<RoomDTO> UpdateRoomAsync(RoomUpdateDTO roomUpdateDTO)
        {
            var roomToUpdate = await _roomRepository.GetByIdAsync(roomUpdateDTO.Id) ?? throw new Exception("No existe ninguna habitación con ese ID");

            // Si se cambió el número de habitación, verificar que no exista otra con ese número
            if (roomToUpdate.RoomNumber != roomUpdateDTO.RoomNumber)
            {
                var roomWithSameNumber = await _roomRepository.GetRoomByNumberAsync(roomUpdateDTO.RoomNumber);
                if (roomWithSameNumber != null && roomWithSameNumber.Id != roomUpdateDTO.Id)
                    throw new Exception($"Ya existe una habitación con el número {roomUpdateDTO.RoomNumber}");
            }

            _mapper.Map(roomUpdateDTO, roomToUpdate);

            roomToUpdate.Status = roomUpdateDTO.IsAvailable ? RoomsStatus.AVAILABLE : RoomsStatus.OCCUPIED;

            await _roomRepository.UpdateAsync(roomToUpdate);

            return _mapper.Map<RoomDTO>(roomToUpdate);
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return false;

            var hasReservations = await _reservationRepository.HasActiveReservationsForRoomAsync(id);
            if (hasReservations)
                throw new Exception("No se puede eliminar una habitación con reservas activas asociadas");

            await _roomRepository.DeleteAsync(room);
            return true;
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateOnly checkIn, DateOnly checkOut)
        {
            return await _reservationRepository.IsRoomAvailableForDatesAsync(roomId, checkIn, checkOut);
        }
    }
}