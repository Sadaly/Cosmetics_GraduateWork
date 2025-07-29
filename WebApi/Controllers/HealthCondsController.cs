using Application.Entity.HealthConds.Commands.ChangeType;
using Application.Entity.HealthConds.Commands.Create;
using Application.Entity.HealthConds.Commands.SoftDelete;
using Application.Entity.HealthConds.Queries;
using Application.Entity.HealthConds.Queries.Get;
using Application.Entity.HealthConds.Queries.GetAll;
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
    public class HealthCondsController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] HealthCondCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut]
        public async Task<IActionResult> ChangeType(
            [FromBody] HealthCondChangeTypeCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{healthCondId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid healthCondId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new HealthCondSoftDeleteCommand(healthCondId), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] HealthCondFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new HealthCondGetAllQuery(HealthCondQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("Take")]
        public async Task<IActionResult> Take(
            [FromQuery] HealthCondFilter filter,
            int StartIndex,
            int Count,
            CancellationToken cancellationToken)
            => (await Sender.Send(new HealthCondGetAllQuery(HealthCondQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{healthCondId:guid}")]
        public async Task<IActionResult> Get(
            Guid healthCondId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new HealthCondGetQuery(HealthCondQueries.GetById(healthCondId)), cancellationToken)).ToActionResult();

    }
}
