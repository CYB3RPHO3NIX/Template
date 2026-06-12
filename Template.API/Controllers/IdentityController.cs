using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.ServiceBus;
using Template.Shared.Models.Requests.Identity;

namespace Template.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IServiceBus _bus;
        public IdentityController(IServiceBus bus)
        {
            _bus = bus;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            Guid? userId = await _bus.Send<Guid?>(new CreateUserCommand
            {
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password
            });
            return Ok(userId);
        }
    }
}
