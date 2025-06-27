using Application.Entity.Users.Commands.UserCreate;
using Application.Entity.Users.Commands.UserLogin;
using Domain.Entity;
using Domain.Errors;
using Domain.Shared;
using Infrastructure.IService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;
using WebApi.Abstractions;
using WebApi.DTO.UserDTO;
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

        //Создание пользователя
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateUser(
            [FromBody] UserCreateCommand command,
            CancellationToken cancellationToken)
            =>  (await Sender.Send(command, cancellationToken)).ToActionResult();

        //Вход в систему
        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(
            [FromBody] UserLoginCommand command,
            CancellationToken cancellationToken)
        {
            Result<string> tokenResult = await Sender.Send(command, cancellationToken);
            if (tokenResult.IsFailure) return tokenResult.ToActionResult();
            
            return _tokenService.GetClaim(
                _tokenService.SetJwtToken(Response, tokenResult.Value), 
                ClaimTypes.NameIdentifier)
                .ToActionResult();
        }


        [Authorize]
        [HttpPost("Logout")]
        public IActionResult LogoutUser()
        {
            _tokenService.DeleteJwtToken(Response);
            return Ok();
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateUserSelf(
            [FromForm] UserUpdateRequest request,
            CancellationToken cancellationToken)
        {
            //сначала нужно получить Id
            var getCurUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (getCurUserId == null) return Result.Failure(WebErrors.UserController.UpdateSelf.EmptyId).ToActionResult();
            var command = new UserUpdateCommand(Guid.Parse(getCurUserId), request.Username, request.Email, request.Password);
            var result = await Sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMe()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(new { userId, email, role });
        }
    }
}
