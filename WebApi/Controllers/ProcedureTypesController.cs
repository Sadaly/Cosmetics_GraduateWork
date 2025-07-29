using Application.Entity.ProcedureTypes.Commands.Create;
using Application.Entity.ProcedureTypes.Commands.SoftDelete;
using Application.Entity.ProcedureTypes.Commands.Update;
using Application.Entity.ProcedureTypes.Queries;
using Application.Entity.ProcedureTypes.Queries.Get;
using Application.Entity.ProcedureTypes.Queries.GetAll;
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
    public class ProcedureTypesController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] ProcedureTypeCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] ProcedureTypeUpdateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{procedureTypeId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid procedureTypeId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ProcedureTypeSoftDeleteCommand(procedureTypeId), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] ProcedureTypeFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ProcedureTypeGetAllQuery(ProcedureTypeQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("Take")]
        public async Task<IActionResult> Take(
            [FromQuery] ProcedureTypeFilter filter,
            int StartIndex,
            int Count,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ProcedureTypeGetAllQuery(ProcedureTypeQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{procedureTypeId:guid}")]
        public async Task<IActionResult> Get(
            Guid procedureTypeId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ProcedureTypeGetQuery(ProcedureTypeQueries.GetById(procedureTypeId)), cancellationToken)).ToActionResult();

    }
}