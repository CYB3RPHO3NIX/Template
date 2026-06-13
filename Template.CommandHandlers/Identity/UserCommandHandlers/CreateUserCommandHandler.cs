using Serilog;
using Serilog.Context;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.CommandHandler;
using Template.Contracts.ServiceBus;
using Template.Database.Abstractions;
using Template.Database.Domain.Entities.Identity;
using Template.Queries.Identity.UserQueries;
using Template.Utilities.Cryptography;

namespace Template.CommandHandlers.Identity.UserCommandHandlers
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid?>
    {
        private readonly IServiceBus _bus;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User, Guid> _userRepository;
        public CreateUserCommandHandler(IServiceBus bus, IUnitOfWork unitOfWork)
        {
            _bus = bus;
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.Repository<User, Guid>();
        }
        public async Task<Guid?> Handle(CreateUserCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling CreateUserCommand for UserName: {UserName}, Email: {Email}", command.UserName, command.Email);
                Guid userId = Guid.Empty;
                Log.Information("Checking if user already exists with UserName: {UserName} or Email: {Email}", command.UserName, command.Email);
                bool userExists = await _bus.Send<bool>(new DoesUserExistQuery
                {
                    TraceId = command.TraceId,
                    Email = command.Email ?? null,
                    UserName = command.UserName ?? null
                });
                if (!userExists)
                {
                    userId = Guid.NewGuid();
                    string passwordSalt = PasswordSaltGenerator.GenerateSalt();
                    await _userRepository.AddAsync(new User
                    {
                        Id = userId,
                        Email = command.Email ?? string.Empty,
                        UserName = command.UserName ?? string.Empty,
                        PasswordHash = HashGenerator.GenerateSHA256Hash(command.Password, passwordSalt),
                        PasswordSalt = passwordSalt,
                        IsActive = true
                    });
                    await _unitOfWork.SaveChangesAsync();
                    Log.Information("User created successfully with UserId: {UserId}", userId);
                }else
                {
                    Log.Information("User already exists with UserName: {UserName} or Email: {Email}", command.UserName, command.Email);
                }
                return userId;
            }
        }
    }
}
