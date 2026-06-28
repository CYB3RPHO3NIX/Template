using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Contracts.QueryHandler;
using Template.Database.Domain.Contexts;
using Template.Database.Domain.Entities;
using Template.Queries.Identity.UserQueries;

namespace Template.QueryHandlers.Identity.UserQueryHandlers
{
    public class DoesUserExistQueryHandler : IQueryHandler<DoesUserExistQuery, bool>
    {
        private readonly TemplateDbContext _dbContext;
        public DoesUserExistQueryHandler(TemplateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DoesUserExistQuery query)
        {
            using (LogContext.PushProperty("TraceId", query.TraceId))
            {
                Log.Information("Handling DoesUserExistQuery for UserName: {UserName}, Email: {Email}", query.UserName, query.Email);

                bool userExists = await _dbContext.Users.AnyAsync(u => u.Username == query.UserName || u.Email == query.Email || u.Id == query.UserId);

                Log.Information("User existence check result: {UserExists}", userExists);
                return userExists;
            }
        }
    }
}
