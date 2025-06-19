using Application.Entity.Users.Commands.UserCreate;
using Application.Entity.Users.Commands.UserLogin;
using Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Abstractions;
using WebAPI.Extensions;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ApiController
    {
        public UsersController(ISender sender) : base(sender) { }

        [HttpPost("Registration")]
        public async Task<IActionResult> CreateUser(
            [FromBody] UserCreateCommand command,
            CancellationToken cancellationToken)
            =>  (await Sender.Send(command, cancellationToken)).ToActionResult();

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(
            [FromBody] UserLoginCommand command,
            CancellationToken cancellationToken)
        {
            Result<string> tokenResult = await Sender.Send(command, cancellationToken);

            if (tokenResult.IsFailure) return tokenResult.ToActionResult();

            SetToken(tokenResult.Value);

            string? userId = JwtHelper.GetClaim(tokenResult.Value, ClaimTypes.NameIdentifier);

            return Ok(userId);
        }

        [Authorize]
        [HttpPost("Logout")]
        public IActionResult LogoutUser()
        {
            Response.Cookies.Delete("access_token");
            return Ok();
        }

        private void SetToken(Result<string> token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            };

            Response.Cookies.Delete("access_token");
            Response.Cookies.Append("access_token", token.Value, cookieOptions);

            string? userId = JwtHelper.GetClaim(token.Value, "sub");
        }
    }
}
