using Application.Entity.Procedures.Commands.AssignDoctor;
using Application.Entity.Procedures.Commands.ChangeType;
using Application.Entity.Procedures.Commands.Create;
using Application.Entity.Procedures.Commands.RemoveDoctor;
using Application.Entity.Procedures.Commands.SoftDelete;
using Application.Entity.Procedures.Commands.UpdateDate;
using Application.Entity.Procedures.Queries;
using Application.Entity.Procedures.Queries.Get;
using Application.Entity.Procedures.Queries.GetAll;
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
    public class ProceduresController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] ProcedureCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut("ChangeType")]
        public async Task<IActionResult> ChangeType(
            [FromBody] ProcedureChangeTypeCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut("AssignDoctor")]
        public async Task<IActionResult> AssignDoctor(
            [FromBody] ProcedureAssignDoctorCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut("RemoveDoctor")]
        public async Task<IActionResult> RemoveDoctor(
            [FromBody] ProcedureRemoveDoctorCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut("UpdateDate")]
        public async Task<IActionResult> UpdateDate(
            [FromBody] ProcedureUpdateDateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{procedureId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid procedureId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ProcedureSoftDeleteCommand(procedureId), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] ProcedureFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ProcedureGetAllQuery(ProcedureQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet]
        public async Task<IActionResult> Take(
            [FromQuery] ProcedureFilter filter,
            int StartIndex,
            int Count,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ProcedureGetAllQuery(ProcedureQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{procedureId:guid}")]
        public async Task<IActionResult> Get(
            Guid procedureId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new ProcedureGetQuery(ProcedureQueries.GetById(procedureId)), cancellationToken)).ToActionResult();

    }
}
