using Mist.Auth.Domain.Validators.UserValidators;
using System;

namespace Mist.Auth.Domain.Commands.UserCommands
{
    public class RegistrarUserCommand : UserCommand<RegistrarUserCommand>
    {
        public RegistrarUserCommand()
            : base(Guid.Empty) { }

        public override bool IsValid()
        {
            ValidationResult = new RegistrarUserCommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
