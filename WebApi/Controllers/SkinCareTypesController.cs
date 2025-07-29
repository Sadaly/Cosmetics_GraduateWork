using Application.Entity.SkinCareTypes.Commands.Create;
using Application.Entity.SkinCareTypes.Commands.SoftDelete;
using Application.Entity.SkinCareTypes.Commands.Update;
using Application.Entity.SkinCareTypes.Queries;
using Application.Entity.SkinCareTypes.Queries.Get;
using Application.Entity.SkinCareTypes.Queries.GetAll;
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
    public class SkinCareTypesController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] SkinCareTypeCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] SkinCareTypeUpdateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{skinCareTypeId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid skinCareTypeId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinCareTypeSoftDeleteCommand(skinCareTypeId), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] SkinCareTypeFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinCareTypeGetAllQuery(SkinCareTypeQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("Take")]
        public async Task<IActionResult> Take(
            [FromQuery] SkinCareTypeFilter filter,
            int StartIndex,
            int Count,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinCareTypeGetAllQuery(SkinCareTypeQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{skinCareTypeId:guid}")]
        public async Task<IActionResult> Get(
            Guid skinCareTypeId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinCareTypeGetQuery(SkinCareTypeQueries.GetById(skinCareTypeId)), cancellationToken)).ToActionResult();

    }
}