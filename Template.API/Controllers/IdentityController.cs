using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.ServiceBus;
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
        [HttpPost("user/create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var validationResult = request.Validate();
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            Guid? userId = await _bus.Send<Guid?>(new CreateUserCommand
            {
                TraceId = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password
            });
            return Ok(userId);
        }
        [HttpPatch("user/{userId}/update")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
        {
            var validationResult = request.Validate();
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            bool result = await _bus.Send<bool>(new UpdateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userId,
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password,
                IsActive = request.IsActive
            });
            return Ok(result);
        }
    }
}
