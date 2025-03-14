using AutoMapper;
using Moq;
using ReservacionesHotel.Interfaces.Repositories;
using ReservacionesHotel.Interfaces.Services;
using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;
using ReservacionesHotel.Services;
using System;
using Xunit;

namespace ReservacionesHotel.Tests
{
    public class RoomServiceTest
    {
        private readonly Mock<IRoomRepository> _roomRepositoryMock;
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IRoomService _roomService;

        public RoomServiceTest()
        {
            _roomRepositoryMock = new Mock<IRoomRepository>();
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _mapperMock = new Mock<IMapper>();
            _roomService = new RoomService(_roomRepositoryMock.Object, _reservationRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateRoomAsync_ShouldThrowException_WhenRoomNumberAlreadyExists()
        {
            // Arrange
            var roomCreateDTO = new RoomCreateDTO { RoomNumber = "101", IsAvailable = true };
            _roomRepositoryMock.Setup(repo => repo.GetRoomByNumberAsync(roomCreateDTO.RoomNumber))
                .ReturnsAsync(new Rooms());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _roomService.CreateRoomAsync(roomCreateDTO));
            Assert.Equal($"Ya existe una habitación con el número {roomCreateDTO.RoomNumber}", exception.Message);
        }

        [Fact]
        public async Task CreateRoomAsync_ShouldCreateRoom_WhenRoomNumberDoesNotExist()
        {
            // Arrange
            var roomCreateDTO = new RoomCreateDTO { RoomNumber = "101", IsAvailable = true };
            var roomEntity = new Rooms { RoomNumber = "101", Status = RoomsStatus.AVAILABLE };
            var roomDTO = new RoomDTO { RoomNumber = "101", IsAvailable = true };

            _roomRepositoryMock.Setup(repo => repo.GetRoomByNumberAsync(roomCreateDTO.RoomNumber))
                .ReturnsAsync((Rooms)null);

            _mapperMock.Setup(mapper => mapper.Map<Rooms>(roomCreateDTO))
                .Returns(roomEntity);
            _roomRepositoryMock.Setup(repo => repo.AddAsync(roomEntity))
                .Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map<RoomDTO>(roomEntity))
                .Returns(roomDTO);

            // Act
            var result = await _roomService.CreateRoomAsync(roomCreateDTO);

            // Assert
            Assert.Equal(roomDTO.RoomNumber, result.RoomNumber);
            Assert.Equal(roomDTO.IsAvailable, result.IsAvailable);
            _roomRepositoryMock.Verify(repo => repo.AddAsync(roomEntity), Times.Once);
        }
    }
}
