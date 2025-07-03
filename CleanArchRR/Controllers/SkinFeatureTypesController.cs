using Application.Entity.SkinFeatureTypes.Commands.Create;
using Application.Entity.SkinFeatureTypes.Commands.SoftDelete;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;
using WebApi.Extensions;
using WebApi.Policies;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class SkinFeatureTypesController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(
            [FromBody] SkinFeatureTypeCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete]
        public async Task<IActionResult> GetAll(
            [FromBody] SkinFeatureTypeSoftDeleteCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

    }
}