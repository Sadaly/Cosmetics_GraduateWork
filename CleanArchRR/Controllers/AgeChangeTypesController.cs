using Application.Entity.AgeChangeTypes.Commands.Create;
using Application.Entity.AgeChangeTypes.Commands.SoftDelete;
using Application.Entity.AgeChangeTypes.Commands.Update;
using Application.Entity.AgeChangeTypes.Queries;
using Application.Entity.AgeChangeTypes.Queries.Get;
using Application.Entity.AgeChangeTypes.Queries.GetAll;
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
    public class AgeChangeTypesController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] AgeChangeTypeCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] AgeChangeTypeUpdateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{ageChangeTypeId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid ageChangeTypeId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new AgeChangeTypeSoftDeleteCommand(ageChangeTypeId), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] AgeChangeTypeFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new AgeChangeTypeGetAllQuery(AgeChangeTypeQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{ageChangeTypeId:guid}")]
        public async Task<IActionResult> Get(
            Guid ageChangeTypeId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new AgeChangeTypeGetQuery(AgeChangeTypeQueries.GetById(ageChangeTypeId)), cancellationToken)).ToActionResult();

    }
}