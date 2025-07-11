using Application.Entity.PatientSpecificses.Commands.Create;
using Application.Entity.PatientSpecificses.Commands.SoftDelete;
using Application.Entity.PatientSpecificses.Commands.Update;
using Application.Entity.PatientSpecificses.Queries;
using Application.Entity.PatientSpecificses.Queries.Get;
using Application.Entity.PatientSpecificses.Queries.GetAll;
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
    public class PatientSpecificsesController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] PatientSpecificsCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] PatientSpecificsUpdateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{patientspecificsId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid patientspecificsId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new PatientSpecificsSoftDeleteCommand(patientspecificsId), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] PatientSpecificsFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet]
        public async Task<IActionResult> Take(
            [FromQuery] PatientSpecificsFilter filter,
            int StartIndex,
            int Count,
            CancellationToken cancellationToken)
            => (await Sender.Send(new PatientSpecificsGetAllQuery(PatientSpecificsQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{patientspecificsId:guid}")]
        public async Task<IActionResult> Get(
            Guid patientspecificsId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new PatientSpecificsGetQuery(PatientSpecificsQueries.GetById(patientspecificsId)), cancellationToken)).ToActionResult();

    }
}
