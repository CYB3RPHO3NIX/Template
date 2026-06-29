using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Contracts.QueryHandler;
using Template.Database.Domain.Contexts;
using Template.Database.Domain.Entities;
using Template.Queries.Identity.UserQueries;
using Template.Shared.Models.DTOs.Identity;

namespace Template.QueryHandlers.Identity.UserQueryHandlers
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDTO?>
    {
        private readonly TemplateDbContext _dbContext;
        public GetUserByIdQueryHandler(TemplateDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<UserDTO?> Handle(GetUserByIdQuery query)
        {
            using (LogContext.PushProperty("TraceId", query.TraceId))
            {
                Log.Information("Handling GetUsersByIdsQueryHandler for UserId: {UserId}", query.UserId);
                var user = await _dbContext.Users.Where(x => x.UserId == query.UserId).FirstOrDefaultAsync();
                if (user != null)
                {
                    return user.Adapt<UserDTO>();
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
