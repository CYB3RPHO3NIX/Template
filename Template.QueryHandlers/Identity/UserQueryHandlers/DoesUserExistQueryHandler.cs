using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Template.Contracts.QueryHandler;
using Template.Database.Abstractions;
using Template.Database.Domain.Entities.Identity;
using Template.Queries.Identity.UserQueries;
using Template.Contracts.ServiceBus;

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
            bool userExists = await _userRepository.Query(true).AnyAsync(u => u.UserName == query.UserName || u.Email == query.Email);
            return userExists;
        }
    }
}
