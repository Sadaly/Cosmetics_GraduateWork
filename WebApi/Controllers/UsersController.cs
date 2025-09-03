using Application.Entity.Users.Commands.Create;
using Application.Entity.Users.Commands.Login;
using Application.Entity.Users.Commands.SoftDelete;
using Application.Entity.Users.Commands.Update;
using Application.Entity.Users.Queries;
using Application.Entity.Users.Queries.Get;
using Application.Entity.Users.Queries.GetAll;
using Domain.Errors;
using Domain.Shared;
using Domain.SupportData.Filters;
using Infrastructure.IService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Abstractions;
using WebApi.DTO.UserDTO;
using WebApi.Extensions;
using WebApi.Policies;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	public class UsersController(ISender sender, ITokenService tokenService) : ApiController(sender)
	{
		//Создание пользователя
		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpPost("Register")]
		public async Task<IActionResult> Create(
			[FromBody] UserCreateCommand command,
			CancellationToken cancellationToken)
			=> (await Sender.Send(command, cancellationToken)).ToActionResult();

		//Вход в систему
		[HttpPost("Login")]
		public async Task<IActionResult> Login(
			[FromBody] UserLoginCommand command,
			CancellationToken cancellationToken)
		{
			Result<string> tokenResult = await Sender.Send(command, cancellationToken);
			if (tokenResult.IsFailure) return tokenResult.ToActionResult();

			return tokenService.GetClaim(
				tokenService.SetJwtToken(Response, tokenResult.Value),
				ClaimTypes.NameIdentifier)
				.ToActionResult();
		}

		[HttpPost("Logout")]
		public IActionResult Logout()
		{
			tokenService.DeleteJwtToken(Response);
			return Ok();
		}

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpPut("Self")]
		public async Task<IActionResult> UpdateSelf(
			[FromForm] UserUpdateRequest request,
			CancellationToken cancellationToken)
		{
			//сначала нужно получить Id
			var claim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (claim == null) return Result.Failure(WebErrors.UserController.UpdateSelf.EmptyId).ToActionResult();
			string getCurUserId = claim.Value;

			var command = new UserUpdateCommand(Guid.Parse(getCurUserId), request.Username, request.Email, request.Password);

			var token = await Sender.Send(command, cancellationToken);
			if (token.IsFailure) return token.ToActionResult();

			return tokenService.GetClaim(
				tokenService.SetJwtToken(Response, token.Value),
				ClaimTypes.NameIdentifier)
				.ToActionResult();
		}

		[HttpGet("Me")]
		[Authorize]
		public IActionResult GetSelf()
		{
			var role = User.FindFirst(ClaimTypes.Role)?.Value;
			var email = User.FindFirst(ClaimTypes.Email)?.Value;
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return Ok(new { userId, email, role });
		}

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("{userId:guid}")]
		public async Task<IActionResult> GetById(
			Guid userId,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new UserGetQuery(UserQueries.GetById(userId)), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("All")]
		public async Task<IActionResult> GetAll(
			[FromQuery] UserFilter filter,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new UserGetAllQuery(UserQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("Take")]
		public async Task<IActionResult> Take(
			[FromQuery] UserFilter filter,
			int StartIndex,
			int Count,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new UserGetAllQuery(UserQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpDelete("{userId:guid}")]
		public async Task<IActionResult> RemoveById(
			Guid userId,
			CancellationToken cancellationToken)
		{
			var claim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (claim == null) return Result.Failure(WebErrors.UserController.RemoveById.EmptyId).ToActionResult();
			string getCurUserId = claim.Value;
			if (getCurUserId == userId.ToString())
				return Result.Failure(WebErrors.UserController.RemoveById.SelfDeleteFobbiden).ToActionResult();
			return (await Sender.Send(new UserSoftDeleteCommand(userId), cancellationToken)).ToActionResult();
		}
	}
}
