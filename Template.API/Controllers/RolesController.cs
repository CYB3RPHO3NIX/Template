using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Commands.Identity.RoleCommands;
using Template.Contracts.ServiceBus;
using Template.Queries.Identity.RoleQueries;
using Template.Shared.Models.DTOs.Identity;
using Template.Shared.Models.Exceptions;
using Template.Shared.Models.Pagination;
using Template.Shared.Models.Requests.Identity;

namespace Template.API.Controllers
{
    [Route("api/roles")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IServiceBus _bus;

        public RolesController(IServiceBus bus)
        {
            _bus = bus;
        }

        [HttpGet]
        public async Task<IActionResult> ListRoles(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "RoleName",
            [FromQuery] bool sortDescending = false,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            var query = new ListRolesQuery
            {
                TraceId = Guid.NewGuid(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending,
                SearchTerm = searchTerm,
                IsActive = isActive
            };

            var result = await _bus.Send<PaginatedResponse<RoleDTO>>(query);

            return Ok(new
            {
                success = true,
                data = result,
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRole(Guid roleId)
        {
            var role = await _bus.Send<RoleDTO?>(new GetRoleByIdQuery
            {
                TraceId = Guid.NewGuid(),
                RoleId = roleId
            });

            if (role == null)
            {
                throw new ResourceNotFoundException("Role", roleId);
            }

            return Ok(new
            {
                success = true,
                data = role,
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var validationResult = request.Validate();
            if (!validationResult.IsValid)
            {
                var errors = new Dictionary<string, List<string>>
                {
                    { "validationErrors", validationResult.Errors }
                };
                throw new ValidationException(errors);
            }

            var roleId = await _bus.Send<Guid?>(new CreateRoleCommand
            {
                TraceId = Guid.NewGuid(),
                RoleName = request.RoleName,
                Description = request.Description,
                IsSystem = request.IsSystem
            });

            if (roleId == null)
            {
                throw new BusinessLogicException("Role creation failed", "ROLE_CREATION_FAILED");
            }

            return Ok(new
            {
                success = true,
                data = new { roleId },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPatch("{roleId}")]
        public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] UpdateRoleRequest request)
        {
            var validationResult = request.Validate();
            if (!validationResult.IsValid)
            {
                var errors = new Dictionary<string, List<string>>
                {
                    { "validationErrors", validationResult.Errors }
                };
                throw new ValidationException(errors);
            }

            var result = await _bus.Send<bool>(new UpdateRoleCommand
            {
                TraceId = Guid.NewGuid(),
                RoleId = roleId,
                RoleName = request.RoleName,
                Description = request.Description,
                IsActive = request.IsActive
            });

            if (!result)
            {
                throw new BusinessLogicException("Role update failed", "ROLE_UPDATE_FAILED");
            }

            return Ok(new
            {
                success = true,
                data = new { updated = true },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(Guid roleId)
        {
            var result = await _bus.Send<bool>(new DeleteRoleCommand
            {
                TraceId = Guid.NewGuid(),
                RoleId = roleId
            });

            if (!result)
            {
                throw new ResourceNotFoundException("Role", roleId);
            }

            return Ok(new
            {
                success = true,
                data = new { deleted = true },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPost("{roleId}/assign-user/{userId}")]
        public async Task<IActionResult> AssignRoleToUser(Guid roleId, Guid userId)
        {
            var userRoleId = await _bus.Send<Guid?>(new AssignRoleToUserCommand
            {
                TraceId = Guid.NewGuid(),
                RoleId = roleId,
                UserId = userId
            });

            if (userRoleId == null)
            {
                throw new BusinessLogicException("Failed to assign role to user", "ROLE_ASSIGNMENT_FAILED");
            }

            return Ok(new
            {
                success = true,
                data = new { userRoleId },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPost("{roleId}/remove-user/{userId}")]
        public async Task<IActionResult> RemoveRoleFromUser(Guid roleId, Guid userId)
        {
            var result = await _bus.Send<bool>(new RemoveRoleFromUserCommand
            {
                TraceId = Guid.NewGuid(),
                RoleId = roleId,
                UserId = userId
            });

            if (!result)
            {
                throw new BusinessLogicException("Failed to remove role from user", "ROLE_REMOVAL_FAILED");
            }

            return Ok(new
            {
                success = true,
                data = new { removed = true },
                traceId = HttpContext.TraceIdentifier
            });
        }
    }
}
