using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductReviewer.Application.Segregation.Auth.Commands;
using ProductReviewer.Application.Segregation.Auth.Queries;

namespace ProductReviewer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        /// <summary>
        /// Authenticates a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="query">The login request contains username and password</param>
        /// <returns>Return JWT Bearer token for futher endpoint access.</returns>

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequestQuery query)
        {
            var result = await Mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Creates a new user with the provided details with User role.
        /// </summary>
        /// <param name="command">The request contains username, password, email & displayname</param>
        /// <returns>Returns success/failure message</returns>
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await Mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Assigns a role to a user. This is an Admin-only operation.
        /// </summary>
        /// <param name="command">Request contains userid and RoleName ["Admin", "Reviewer", "User"]</param>
        /// <returns>Returns success/failure message</returns>

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleToUserCommand command)
        {
            var result = await Mediator.Send(command);
            return result.ToActionResult();
        }
    }
}
