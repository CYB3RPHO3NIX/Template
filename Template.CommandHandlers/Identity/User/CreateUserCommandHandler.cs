using Template.Commands.Identity.User;
using Template.Contracts.CommandHandler;
using Template.Contracts.ServiceBus;

namespace Template.CommandHandlers.Identity.User
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid?>
    {
        private readonly IServiceBus _bus;
        public CreateUserCommandHandler(IServiceBus bus)
        {
            _bus = bus;
        }
        public async Task<Guid?> Handle(CreateUserCommand command)
        {
            // Implement the logic to create a user
            throw new NotImplementedException();
        }
    }
}
