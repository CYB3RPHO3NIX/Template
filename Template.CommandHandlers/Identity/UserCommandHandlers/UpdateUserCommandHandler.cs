using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Text;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.CommandHandler;
using Template.Contracts.ServiceBus;
using Template.Database.Abstractions;
using Template.Database.Domain.Entities.Identity;
using Template.Utilities.Cryptography;

namespace Template.CommandHandlers.Identity.UserCommandHandlers
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, bool>
    {
        private readonly IServiceBus _bus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User, Guid> _userRepository;
        public UpdateUserCommandHandler(IServiceBus bus, IUnitOfWork unitOfWork)
        {
            _bus = bus;
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.Repository<User, Guid>();
        }
        public async Task<bool> Handle(UpdateUserCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling UpdateUserCommand for UserId: {UserId}", command.UserId);
                
                Log.Information("Checking if user already exists with UserId: {UserId}", command.UserId);

                var user = await _userRepository.GetByIdAsync(command.UserId);

                if(user != null)
                {
                    if (!string.IsNullOrEmpty(command?.UserName))
                    {
                        Log.Information("Updating UserName for UserId: {UserId}", command.UserId);
                        user.UserName = command.UserName;
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
                    if (command.IsActive.HasValue)
                    {
                        Log.Information("Updating IsActive Flag for UserId: {UserId}", command.UserId);
                        user.IsActive = command.IsActive.Value;
                    }
                    _userRepository.Update(user);
                    await _unitOfWork.SaveChangesAsync();
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
