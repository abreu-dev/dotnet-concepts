using Mist.Auth.Domain.Entities;
using System;

namespace Mist.Auth.Domain.Commands.UserCommands
{
    public abstract class UserCommand<T> : Command 
        where T : UserCommand<T>
    {
        public User Entity { get; set; }

        protected UserCommand(Guid aggregateId)
            : base(aggregateId) { }
    }
}
