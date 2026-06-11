using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Template.Contracts.QueryHandler;
using Template.Database.Abstractions;
using Template.Database.Domain.Entities.Identity;
using Template.Queries.Identity.User;
using Template.Contracts.ServiceBus;

namespace Template.QueryHandlers.Identity.User
{
    public class DoesUserExistQueryHandler : IQueryHandler<DoesUserExistQuery, bool>
    {
        public DoesUserExistQueryHandler()
        {
            
        }

        public async Task<bool> Handle(DoesUserExistQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
