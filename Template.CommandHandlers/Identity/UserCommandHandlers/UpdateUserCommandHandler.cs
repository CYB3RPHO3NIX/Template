using Serilog;
using Serilog.Context;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.CommandHandler;
using Template.Contracts.ServiceBus;
using Template.Database.Domain.Contexts;
using Template.Database.Domain.Entities;
using Template.Utilities.Cryptography;

namespace Template.CommandHandlers.Identity.UserCommandHandlers
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, bool>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;
        public UpdateUserCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }
        public async Task<bool> Handle(UpdateUserCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling UpdateUserCommand for UserId: {UserId}", command.UserId);
                
                Log.Information("Checking if user already exists with UserId: {UserId}", command.UserId);

                var user = await _dbContext.Users.FindAsync(command.UserId);

                if(user != null)
                {
                    if (!string.IsNullOrEmpty(command?.UserName))
                    {
                        Log.Information("Updating UserName for UserId: {UserId}", command.UserId);
                        user.Username = command.UserName;
                    }
                    if (!string.IsNullOrEmpty(command?.Email))
                    {
                        Log.Information("Updating Email for UserId: {UserId}", command.UserId);
                        user.Email = command.Email;
                    }
                    if (!string.IsNullOrEmpty(command?.Password))
                    {
                        Log.Information("Updating Password for UserId: {UserId}", command.UserId);
                        string passwordSalt = PasswordSaltGenerator.GenerateSalt();
                        user.PasswordSalt = passwordSalt;
                        user.PasswordHash = HashGenerator.GenerateSHA256Hash(command.Password, passwordSalt);
                    }
                    if (command?.IsActive != null)
                    {
                        Log.Information("Updating IsActive Flag for UserId: {UserId}", command.UserId);
                        user.IsActive = command.IsActive.Value;
                    }

                    user.UpdatedOn = DateTime.UtcNow;
                    _dbContext.Users.Update(user);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    Log.Warning("User with UserId: {UserId} not found", command.UserId);
                    return false;
                }
            }
        }
    }
}
