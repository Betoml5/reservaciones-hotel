using AutoMapper;
using Moq;
using ReservacionesHotel.Interfaces.Repositories;
using ReservacionesHotel.Interfaces.Services;
using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;
using ReservacionesHotel.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReservacionesHotel.Tests
{
    public class ReservationServiceTest
    {
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly Mock<IRoomRepository> _roomRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly ReservationService _reservationService;

        public ReservationServiceTest()
        {
            _reservationRepositoryMock = new Mock<IReservationRepository>();
            _roomRepositoryMock = new Mock<IRoomRepository>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _reservationService = new ReservationService(
                _reservationRepositoryMock.Object,
                _roomRepositoryMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task CreateReservationAsync_ShouldThrowException_WhenRoomIsNotAvailable()
        {
            // Arrange
            var reservationCreateDTO = new ReservationCreateDTO
            {
                RoomId = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
                CheckOutDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            _reservationRepositoryMock
                .Setup(repo => repo.IsRoomAvailableForDatesAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _reservationService.CreateReservationAsync(reservationCreateDTO));
        }

        [Fact]
        public async Task CreateReservationAsync_ShouldThrowException_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var reservationCreateDTO = new ReservationCreateDTO
            {
                RoomId = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
                CheckOutDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            _reservationRepositoryMock
                .Setup(repo => repo.IsRoomAvailableForDatesAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(true);

            _currentUserServiceMock
                .Setup(service => service.GetCurrentUserAsync())
                .ReturnsAsync((Users)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _reservationService.CreateReservationAsync(reservationCreateDTO));
        }

        [Fact]
        public async Task CreateReservationAsync_ShouldReturnReservationDTO_WhenReservationIsCreated()
        {
            // Arrange
            var reservationCreateDTO = new ReservationCreateDTO
            {
                RoomId = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
                CheckOutDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            var user = new Users { Id = 1 };
            var reservation = new Reservations { Id = 1, UserId = user.Id, TotalPrice = 100, CreatedAt = DateTime.UtcNow, Status = ReservationsStatus.BOOKED };
            var reservationDTO = new ReservationDTO { Id = 1, UserId = user.Id, TotalPrice = 100 };

            _reservationRepositoryMock
                .Setup(repo => repo.IsRoomAvailableForDatesAsync(It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
                .ReturnsAsync(true);

            _currentUserServiceMock
                .Setup(service => service.GetCurrentUserAsync())
                .ReturnsAsync(user);

            _reservationRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Reservations>()))
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(mapper => mapper.Map<Reservations>(It.IsAny<ReservationCreateDTO>()))
                .Returns(reservation);

            _mapperMock
                .Setup(mapper => mapper.Map<ReservationDTO>(It.IsAny<Reservations>()))
                .Returns(reservationDTO);

            // Act
            var result = await _reservationService.CreateReservationAsync(reservationCreateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reservationDTO.Id, result.Id);
            Assert.Equal(reservationDTO.UserId, result.UserId);
            Assert.Equal(reservationDTO.TotalPrice, result.TotalPrice);
        }
    }
}