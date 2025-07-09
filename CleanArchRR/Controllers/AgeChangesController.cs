using Application.Entity.AgeChanges.Commands.ChangeType;
using Application.Entity.AgeChanges.Commands.Create;
using Application.Entity.AgeChanges.Commands.SoftDelete;
using Application.Entity.AgeChanges.Queries;
using Application.Entity.AgeChanges.Queries.Get;
using Application.Entity.AgeChanges.Queries.GetAll;
using Domain.SupportData.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;
using WebApi.Extensions;
using WebApi.Policies;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AgeChangesController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] AgeChangeCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut]
        public async Task<IActionResult> ChangeType(
            [FromBody] AgeChangeChangeTypeCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{ageChangeId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid ageChangeId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new AgeChangeSoftDeleteCommand(ageChangeId), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] AgeChangeFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new AgeChangeGetAllQuery(AgeChangeQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{ageChangeId:guid}")]
        public async Task<IActionResult> Get(
            Guid ageChangeId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new AgeChangeGetQuery(AgeChangeQueries.GetById(ageChangeId)), cancellationToken)).ToActionResult();

    }
}
