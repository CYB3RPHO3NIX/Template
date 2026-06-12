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
            Guid? userId = await _bus.Send<Guid?>(new CreateUserCommand
            {
                TraceId = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password
            });
            return Ok(userId);
        }
    }
}
