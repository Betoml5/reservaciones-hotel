using AutoMapper;
using ReservacionesHotel.Models.Entities;
using ReservacionesHotel.Models.Entities.DTOs;
using System;

namespace ReservacionesHotel.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Users mappings
            CreateMap<UserCreateDTO, Users>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));

            CreateMap<Users, UserDTO>();
            CreateMap<UserCreateDTO, Users>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            CreateMap<UserUpdateDTO, Users>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Room mappings
            CreateMap<Rooms, RoomDTO>()
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.Status == RoomsStatus.AVAILABLE));
            CreateMap<RoomCreateDTO, Rooms>();
            CreateMap<RoomUpdateDTO, Rooms>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Reservation mappings
            CreateMap<Reservations, ReservationDTO>();

            CreateMap<ReservationCreateDTO, Reservations>()
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())  // Se asigna en el servicio
                .ForMember(dest => dest.Room, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<ReservationUpdateDTO, Reservations>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.RoomId, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Room, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<Reservations, ReservationDetailsDTO>()
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));
        }
    }
}