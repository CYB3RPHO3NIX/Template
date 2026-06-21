using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.QueryHandler;
using Template.Database.Abstractions;
using Template.Database.Domain.Entities.Identity;
using Template.Queries.Identity.UserQueries;
using Template.Shared.Models.Common;
using Template.Shared.Models.DTOs.Identity;

namespace Template.QueryHandlers.Identity.UserQueryHandlers
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDTO?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User, Guid> _userRepository;
        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.Repository<User, Guid>();
        }
        public async Task<UserDTO?> Handle(GetUserByIdQuery query)
        {
            using (LogContext.PushProperty("TraceId", query.TraceId))
            {
                Log.Information("Handling GetUserByIdQuery for UserId: {UserId}", query.UserId);

                var user = await _userRepository.Query(true).Where(x => x.Id == query.UserId).FirstOrDefaultAsync();
                if(user != null)
                {
                    return (new UserDTO
                    {
                        UserId = user.Id,
                        UserName = user.Username,
                        Email = user.Email,
                        IsActive = user.IsActive
                    });
                }
                else
                {
                    Log.Warning("User with UserId: {UserId} not found", query.UserId);
                    return null;
                }
            }
        }
    }
}
