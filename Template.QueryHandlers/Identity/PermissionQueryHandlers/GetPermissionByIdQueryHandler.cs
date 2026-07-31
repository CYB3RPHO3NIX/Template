using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Contracts.QueryHandler;
using Template.Database.Domain.Contexts;
using Template.Queries.Identity.PermissionQueries;
using Template.Shared.Models.DTOs.Identity;

namespace Template.QueryHandlers.Identity.PermissionQueryHandlers
{
    public class GetPermissionByIdQueryHandler : IQueryHandler<GetPermissionByIdQuery, PermissionDTO?>
    {
        private readonly TemplateDbContext _dbContext;

        public GetPermissionByIdQueryHandler(TemplateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PermissionDTO?> Handle(GetPermissionByIdQuery query)
        {
            using (LogContext.PushProperty("TraceId", query.TraceId))
            {
                Log.Information("Handling GetPermissionByIdQuery for PermissionId: {PermissionId}", query.PermissionId);

                var permission = await _dbContext.Permissions
                    .Where(p => p.PermissionId == query.PermissionId)
                    .FirstOrDefaultAsync();

                if (permission != null)
                {
                    Log.Information("Permission found for PermissionId: {PermissionId}", query.PermissionId);
                    return permission.Adapt<PermissionDTO>();
                }
                else
                {
                    Log.Warning("Permission with PermissionId: {PermissionId} not found", query.PermissionId);
                    return null;
                }
            }
        }
    }
}
