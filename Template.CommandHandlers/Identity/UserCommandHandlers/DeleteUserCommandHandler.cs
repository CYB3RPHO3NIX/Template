using Mapster;
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
using Template.Queries.Identity.UserQueries;

namespace Template.CommandHandlers.Identity.UserCommandHandlers
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, bool>
    {
        private readonly IServiceBus _bus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User, Guid> _userRepository;
        public DeleteUserCommandHandler(IServiceBus bus, IUnitOfWork unitOfWork)
        {
            _bus = bus;
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.Repository<User, Guid>();
        }
        public async Task<bool> Handle(DeleteUserCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling DeleteUserCommand for UserId: {UserId}", command.UserId);
                var user = await _bus.Send(new GetUserByIdQuery 
                { 
                    UserId = command.UserId
                });
                if(user == null)
                {
                    Log.Warning("User with id {UserId} does not exist", command.UserId);
                    return false;
                }
                _userRepository.Remove(user.Adapt<User>());
                await _unitOfWork.SaveChangesAsync();
                Log.Information("User with id {UserId} has been deleted", command.UserId);
                return true;
            }
        }
    }
}
