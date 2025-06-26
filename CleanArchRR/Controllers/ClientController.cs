//using Application.Entity.Clients.Commands.ClientCreate;
//using Application.Entity.Clients.Commands.ClientLogin;
//using Domain.Shared;
//using Infrastructure.IService;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Reflection;
//using System.Security.Claims;
//using WebApi.Abstractions;
//using WebApi.Extensions;

//namespace WebApi.Controllers
//{
//    [Route("api/[controller]")]
//    public class ClientsController : ApiController
//    {
//        private readonly ITokenService _tokenService;

//        public ClientsController(ISender sender, ITokenService tokenService) : base(sender)
//        {
//            _tokenService = tokenService;
//        }

//        [HttpPost("Registration")]
//        public async Task<IActionResult> CreateClient(
//            [FromBody] ClientsCreateCommand command,
//            CancellationToken cancellationToken)
//            =>  (await Sender.Send(command, cancellationToken)).ToActionResult();

//        [HttpPost("Login")]
//        public async Task<IActionResult> LoginClient(
//            [FromBody] ClientsLoginCommand command,
//            CancellationToken cancellationToken)
//        {
//            Result<string> tokenResult = await Sender.Send(command, cancellationToken);

//            if (tokenResult.IsFailure) return tokenResult.ToActionResult();

//            _tokenService.SetJwtToken(Response, tokenResult.Value);
            
//            string? ClientId = _tokenService.GetClaim(tokenResult.Value, ClaimTypes.NameIdentifier);

//            return Ok(ClientId);
//        }

//        [Authorize]
//        [HttpPost("Logout")]
//        public IActionResult LogoutClient()
//        {
//            _tokenService.DeleteJwtToken(Response);
//            return Ok();
//        }
//    }
//}
