using FluentValidation;
using Mist.Auth.Domain.Commands.UserCommands;

namespace Mist.Auth.Domain.Validators.UserValidators
{
    public class RegistrarUserCommandValidator : UserCommandValidator<RegistrarUserCommand>
    {
        public RegistrarUserCommandValidator()
        {
            RuleFor(c => c.Entity.Email)
                .NotEmpty()
                .WithMessage("Email obrigatório.")
                .EmailAddress()
                .WithMessage("Email em formato inválido.");

            RuleFor(c => c.Entity.Password)
                .NotEmpty()
                .WithMessage("Senha obrigatória.")
                .Length(5, 15)
                .WithMessage("Senha deve ter entre 5 e 15 caracteres.");
        }
    }
}
