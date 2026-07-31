using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Contracts.QueryHandler;
using Template.Database.Domain.Contexts;
using Template.Queries.Identity.RoleQueries;
using Template.Shared.Models.DTOs.Identity;

namespace Template.QueryHandlers.Identity.RoleQueryHandlers
{
    public class GetRoleByIdQueryHandler : IQueryHandler<GetRoleByIdQuery, RoleDTO?>
    {
        private readonly TemplateDbContext _dbContext;

        public GetRoleByIdQueryHandler(TemplateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RoleDTO?> Handle(GetRoleByIdQuery query)
        {
            using (LogContext.PushProperty("TraceId", query.TraceId))
            {
                Log.Information("Handling GetRoleByIdQuery for RoleId: {RoleId}", query.RoleId);

                var role = await _dbContext.Roles
                    .Where(r => r.RoleId == query.RoleId)
                    .FirstOrDefaultAsync();

                if (role != null)
                {
                    Log.Information("Role found for RoleId: {RoleId}", query.RoleId);
                    return role.Adapt<RoleDTO>();
                }
                else
                {
                    Log.Warning("Role with RoleId: {RoleId} not found", query.RoleId);
                    return null;
                }
            }
        }
    }
}
