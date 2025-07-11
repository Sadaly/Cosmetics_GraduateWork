using Application.Entity.SkinCares.Commands.ChangeType;
using Application.Entity.SkinCares.Commands.Create;
using Application.Entity.SkinCares.Commands.SoftDelete;
using Application.Entity.SkinCares.Queries;
using Application.Entity.SkinCares.Queries.Get;
using Application.Entity.SkinCares.Queries.GetAll;
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
    public class SkinCaresController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] SkinCareCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut]
        public async Task<IActionResult> ChangeType(
            [FromBody] SkinCareChangeTypeCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{skinCareId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid skinCareId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinCareSoftDeleteCommand(skinCareId), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] SkinCareFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinCareGetAllQuery(SkinCareQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet]
        public async Task<IActionResult> Take(
            [FromQuery] SkinCareFilter filter,
            int StartIndex,
            int Count,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinCareGetAllQuery(SkinCareQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{skinCareId:guid}")]
        public async Task<IActionResult> Get(
            Guid skinCareId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinCareGetQuery(SkinCareQueries.GetById(skinCareId)), cancellationToken)).ToActionResult();

    }
}
