using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.CommandHandler;
using Template.Contracts.ServiceBus;
using Template.Database.Domain.Contexts;

namespace Template.CommandHandlers.Identity.UserCommandHandlers
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, bool>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public DeleteUserCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteUserCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling DeleteUserCommand for UserId: {UserId}", command.UserId);

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == command.UserId);
                if (user == null)
                {
                    Log.Warning("User with id {UserId} does not exist", command.UserId);
                    return false;
                }

                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
                Log.Information("User with id {UserId} has been deleted", command.UserId);
                return true;
            }
        }
    }
}
