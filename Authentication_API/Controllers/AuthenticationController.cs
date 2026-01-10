using Authentication_Application.Accounts.Commands.Login;
using Authentication_Application.Accounts.Commands.Register;
using Authentication_Application.DTOs;
using Authentication_Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security;
using System.Security.Claims;

namespace Authentication_API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpHead]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var permissions = User.FindAll("permission").Select(c => c.Value);

            var permissionsString = string.Join(",", permissions);

            Response.Headers.Append("X-User-Id", userId);
            Response.Headers.Append("X-Permissions", permissionsString);

            return Ok();
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var result = await _mediator.Send(new LoginCommand(loginDTO.Email, loginDTO.Password));
            return result != null ? Ok(result) : BadRequest("Something went wrong");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var result = await _mediator.Send(new RegisterCommand(registerDTO.Name, registerDTO.Email, registerDTO.Password, registerDTO.ConfirmPassword));
            return result != null ? Ok(result) : BadRequest("Something went wrong");
        }
    }
}
