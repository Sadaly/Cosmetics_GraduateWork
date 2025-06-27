using Application.Entity.Users.Commands.UserCreate;
using Application.Entity.Users.Commands.UserLogin;
using Domain.Entity;
using Domain.Shared;
using Infrastructure.IService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;
using WebApi.Abstractions;
using WebApi.Extensions;
using WebApi.Policies;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ApiController
    {
        private readonly ITokenService _tokenService;

        public UsersController(ISender sender, ITokenService tokenService) : base(sender)
        {
            _tokenService = tokenService;
        }

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost("Create")]
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

            _tokenService.SetJwtToken(Response, tokenResult.Value);
            
            string? userId = _tokenService.GetClaim(tokenResult.Value, ClaimTypes.NameIdentifier);

            return Ok(userId);
        }

        [Authorize]
        [HttpPost("Logout")]
        public IActionResult LogoutUser()
        {
            _tokenService.DeleteJwtToken(Response);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult>
    }
}
