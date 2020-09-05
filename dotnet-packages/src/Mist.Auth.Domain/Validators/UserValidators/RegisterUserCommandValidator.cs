using FluentValidation;
using Mist.Auth.Domain.Commands.UserCommands;

namespace Mist.Auth.Domain.Validators.UserValidators
{
    public class RegisterUserCommandValidator : UserCommandValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(c => c.Entity.Email)
                .NotEmpty()
                .WithMessage("Email required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(c => c.Entity.Password)
                .NotEmpty()
                .WithMessage("Password required.")
                .Length(5, 15)
                .WithMessage("Password must be between 5 and 15 characters.");
        }
    }
}
