using Microsoft.AspNetCore.Mvc;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.ServiceBus;
using Template.Queries.Identity.UserQueries;
using Template.Shared.Models.DTOs.Identity;
using Template.Shared.Models.Exceptions;
using Template.Shared.Models.Requests.Identity;

namespace Template.API.Controllers
{
    [Route("api/identity")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IServiceBus _bus;

        public IdentityController(IServiceBus bus)
        {
            _bus = bus;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var user = await _bus.Send<UserDTO?>(new GetUserByIdQuery
            {
                TraceId = Guid.NewGuid(),
                UserId = userId
            });

            if (user == null)
            {
                throw new ResourceNotFoundException("User", userId);
            }

            return Ok(new
            {
                success = true,
                data = user,
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPost("user/create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
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

            var userId = await _bus.Send<Guid?>(new CreateUserCommand
            {
                TraceId = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password
            });

            if (userId == null)
            {
                throw new BusinessLogicException("User creation failed", "USER_CREATION_FAILED");
            }

            return Ok(new
            {
                success = true,
                data = new { userId },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpPatch("user/{userId}/update")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
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

            var result = await _bus.Send<bool>(new UpdateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userId,
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password,
                IsActive = request.IsActive
            });

            if (!result)
            {
                throw new BusinessLogicException("User update failed", "USER_UPDATE_FAILED");
            }

            return Ok(new
            {
                success = true,
                data = new { updated = true },
                traceId = HttpContext.TraceIdentifier
            });
        }

        [HttpDelete("user/{userId}/delete")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _bus.Send<bool>(new DeleteUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userId
            });

            if (!result)
            {
                throw new ResourceNotFoundException("User", userId);
            }

            return Ok(new
            {
                success = true,
                data = new { deleted = true },
                traceId = HttpContext.TraceIdentifier
            });
        }
    }
}
