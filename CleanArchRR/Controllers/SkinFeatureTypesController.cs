using Application.Entity.SkinFeatureTypes.Commands.Create;
using Application.Entity.SkinFeatureTypes.Commands.SoftDelete;
using Application.Entity.SkinFeatureTypes.Commands.Update;
using Application.Entity.SkinFeatureTypes.Queries;
using Application.Entity.SkinFeatureTypes.Queries.Get;
using Application.Entity.SkinFeatureTypes.Queries.GetAll;
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
    public class SkinFeatureTypesController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(
            [FromBody] SkinFeatureTypeCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] SkinFeatureTypeUpdateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete]
        public async Task<IActionResult> RemoveById(
            Guid id,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinFeatureTypeSoftDeleteCommand(id), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] SkinFeatureTypeFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinFeatureTypeGetAllQuery(SkinFeatureTypeQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{skinFeatureId:guid}")]
        public async Task<IActionResult> Get(
            Guid skinFeatureId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinFeatureTypeGetQuery(SkinFeatureTypeQueries.GetById(skinFeatureId)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("Take")]
        public async Task<IActionResult> Take(
            [FromQuery] SkinFeatureTypeFilter filter,
            int StartIndex,
            int Count,
            CancellationToken cancellationToken)
            => (await Sender.Send(new SkinFeatureTypeGetAllQuery(SkinFeatureTypeQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

    }
}