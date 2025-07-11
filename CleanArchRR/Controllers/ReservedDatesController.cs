using Application.Entity.ReservedDates.Commands.Create;
using Application.Entity.ReservedDates.Commands.SoftDelete;
using Application.Entity.ReservedDates.Queries;
using Application.Entity.ReservedDates.Queries.Get;
using Application.Entity.ReservedDates.Queries.GetAll;
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
    public class ReservedDatesController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] ReservedDateCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{reserveddateId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid reserveddateId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ReservedDateSoftDeleteCommand(reserveddateId), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] ReservedDateFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ReservedDateGetAllQuery(ReservedDateQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("Take")]
        public async Task<IActionResult> Take(
            [FromQuery] ReservedDateFilter filter,
            int StartIndex,
            int Count,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ReservedDateGetAllQuery(ReservedDateQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{reserveddateId:guid}")]
        public async Task<IActionResult> Get(
            Guid reserveddateId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ReservedDateGetQuery(ReservedDateQueries.GetById(reserveddateId)), cancellationToken)).ToActionResult();

    }
}
