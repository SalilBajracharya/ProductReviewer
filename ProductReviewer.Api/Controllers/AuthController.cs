using Microsoft.AspNetCore.Mvc;
using ProductReviewer.Application.Segregation.Auth.Commands;
using ProductReviewer.Application.Segregation.Auth.Queries;

namespace ProductReviewer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate(LoginRequestQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(AssignRoleToUserCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
