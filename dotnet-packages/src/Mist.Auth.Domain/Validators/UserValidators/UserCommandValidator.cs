using FluentValidation;
using Mist.Auth.Domain.Commands.UserCommands;

namespace Mist.Auth.Domain.Validators.UserValidators
{
    public abstract class UserCommandValidator<T> : AbstractValidator<T> where T : UserCommand<T> { }
}
