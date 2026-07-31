using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Commands.Identity.PermissionCommands;
using Template.Contracts.ServiceBus;
using Template.Queries.Identity.PermissionQueries;
using Template.Shared.Models.DTOs.Identity;
using Template.Shared.Models.Exceptions;
using Template.Shared.Models.Pagination;
using Template.Shared.Models.Requests.Identity;

namespace Template.API.Controllers
{
    [Route("api/permissions")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IServiceBus _bus;

        public PermissionsController(IServiceBus bus)
        {
            _bus = bus;
        }

        [HttpGet]
        public async Task<IActionResult> ListPermissions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "PermissionName",
            [FromQuery] bool sortDescending = false,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? category = null,
            [FromQuery] bool? isActive = null)
        {
            var query = new ListPermissionsQuery
            {
                TraceId = Guid.NewGuid(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending,
                SearchTerm = searchTerm,
                Category = category,
                IsActive = isActive
            };

            var result = await _bus.Send<PaginatedResponse<PermissionDTO>>(query);

            return Ok(new
            {
                success = true,
                data = result,
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpGet("{permissionId}")]
        public async Task<IActionResult> GetPermission(Guid permissionId)
        {
            var permission = await _bus.Send<PermissionDTO?>(new GetPermissionByIdQuery
            {
                TraceId = Guid.NewGuid(),
                PermissionId = permissionId
            });

            if (permission == null)
            {
                throw new ResourceNotFoundException("Permission", permissionId);
            }

            return Ok(new
            {
                success = true,
                data = permission,
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
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

            var permissionId = await _bus.Send<Guid?>(new CreatePermissionCommand
            {
                TraceId = Guid.NewGuid(),
                PermissionName = request.PermissionName,
                PermissionCode = request.PermissionCode,
                Description = request.Description,
                Category = request.Category,
                ParentPermissionId = request.ParentPermissionId
            });

            if (permissionId == null)
            {
                throw new BusinessLogicException("Permission creation failed", "PERMISSION_CREATION_FAILED");
            }

            return Ok(new
            {
                success = true,
                data = new { permissionId },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPatch("{permissionId}")]
        public async Task<IActionResult> UpdatePermission(Guid permissionId, [FromBody] UpdatePermissionRequest request)
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

            var result = await _bus.Send<bool>(new UpdatePermissionCommand
            {
                TraceId = Guid.NewGuid(),
                PermissionId = permissionId,
                PermissionName = request.PermissionName,
                Description = request.Description,
                Category = request.Category,
                IsActive = request.IsActive
            });

            if (!result)
            {
                throw new BusinessLogicException("Permission update failed", "PERMISSION_UPDATE_FAILED");
            }

            return Ok(new
            {
                success = true,
                data = new { updated = true },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpDelete("{permissionId}")]
        public async Task<IActionResult> DeletePermission(Guid permissionId)
        {
            var result = await _bus.Send<bool>(new DeletePermissionCommand
            {
                TraceId = Guid.NewGuid(),
                PermissionId = permissionId
            });

            if (!result)
            {
                throw new ResourceNotFoundException("Permission", permissionId);
            }

            return Ok(new
            {
                success = true,
                data = new { deleted = true },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPost("{permissionId}/assign-role/{roleId}")]
        public async Task<IActionResult> AssignPermissionToRole(Guid permissionId, Guid roleId, [FromQuery] bool isAllowed = true)
        {
            var rolePermissionId = await _bus.Send<Guid?>(new AssignPermissionToRoleCommand
            {
                TraceId = Guid.NewGuid(),
                PermissionId = permissionId,
                RoleId = roleId,
                IsAllowed = isAllowed
            });

            if (rolePermissionId == null)
            {
                throw new BusinessLogicException("Failed to assign permission to role", "PERMISSION_ASSIGNMENT_FAILED");
            }

            return Ok(new
            {
                success = true,
                data = new { rolePermissionId },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPost("{permissionId}/remove-role/{roleId}")]
        public async Task<IActionResult> RemovePermissionFromRole(Guid permissionId, Guid roleId)
        {
            var result = await _bus.Send<bool>(new RemovePermissionFromRoleCommand
            {
                TraceId = Guid.NewGuid(),
                PermissionId = permissionId,
                RoleId = roleId
            });

            if (!result)
            {
                throw new BusinessLogicException("Failed to remove permission from role", "PERMISSION_REMOVAL_FAILED");
            }

            return Ok(new
            {
                success = true,
                data = new { removed = true },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPost("{permissionId}/assign-user/{userId}")]
        public async Task<IActionResult> AssignPermissionToUser(Guid permissionId, Guid userId, [FromQuery] bool isAllowed = true, [FromQuery] string? reason = null)
        {
            var userPermissionId = await _bus.Send<Guid?>(new AssignPermissionToUserCommand
            {
                TraceId = Guid.NewGuid(),
                PermissionId = permissionId,
                UserId = userId,
                IsAllowed = isAllowed,
                Reason = reason
            });

            if (userPermissionId == null)
            {
                throw new BusinessLogicException("Failed to assign permission to user", "PERMISSION_ASSIGNMENT_FAILED");
            }

            return Ok(new
            {
                success = true,
                data = new { userPermissionId },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPost("{permissionId}/remove-user/{userId}")]
        public async Task<IActionResult> RemovePermissionFromUser(Guid permissionId, Guid userId)
        {
            var result = await _bus.Send<bool>(new RemovePermissionFromUserCommand
            {
                TraceId = Guid.NewGuid(),
                PermissionId = permissionId,
                UserId = userId
            });

            if (!result)
            {
                throw new BusinessLogicException("Failed to remove permission from user", "PERMISSION_REMOVAL_FAILED");
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
