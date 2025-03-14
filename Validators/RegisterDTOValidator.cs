using FluentValidation;
using ReservacionesHotel.Interfaces.Repositories;
using ReservacionesHotel.Models.Entities.DTOs;

namespace ReservacionesHotel.Validators
{
    public class UserCreateValidator : AbstractValidator<UserCreateDTO>
    {
        private readonly IUserRepository _userRepository;

        public UserCreateValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("El formato del email no es válido.")
                .MustAsync(async (email, cancellation) =>
                {
                    var existingUser = await _userRepository.GetUserByEmailAsync(email);
                    return existingUser == null;
                }).WithMessage("El email ya está registrado.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número.")
                .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe contener al menos un caracter especial.");

            // Optional validation for Role
            When(x => !string.IsNullOrEmpty(x.Role), () =>
            {
                RuleFor(x => x.Role)
                    .Must(role => role == "User" || role == "Admin")
                    .WithMessage("El rol debe ser 'User' o 'Admin'.");
            });
        }
    }
}