using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Template.Contracts.QueryHandler;
using Template.Database.Abstractions;
using Template.Database.Domain.Entities.Identity;
using Template.Queries.Identity.UserQueries;
using Template.Contracts.ServiceBus;
using Serilog;
using Serilog.Context;
using Template.Shared.Models.Common;

namespace Template.QueryHandlers.Identity.UserQueryHandlers
{
    public class DoesUserExistQueryHandler : IQueryHandler<DoesUserExistQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User, Guid> _userRepository;
        public DoesUserExistQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.Repository<User, Guid>();
        }

        public async Task<bool> Handle(DoesUserExistQuery query)
        {
            using (LogContext.PushProperty("TraceId", query.TraceId))
            {
                Log.Information("Handling DoesUserExistQuery for UserName: {UserName}, Email: {Email}", query.UserName, query.Email);

                bool userExists = await _userRepository.Query(true).AnyAsync(u => u.Username == query.UserName || u.Email == query.Email || u.Id == query.UserId);

                Log.Information("User existence check result: {UserExists}", userExists);
                return userExists;
            }
        }
    }
}
