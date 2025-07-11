using Application.Entity.Patients.Commands.Create;
using Application.Entity.Patients.Commands.SoftDelete;
using Application.Entity.Patients.Commands.Update;
using Application.Entity.Patients.Queries;
using Application.Entity.Patients.Queries.GetAll;
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
    public class PatientsController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] PatientCreateCommand command,
            CancellationToken cancellationToken)
            =>  (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] PatientUpdateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] PatientFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new PatientGetAllQuery(PatientQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet]
        public async Task<IActionResult> Take(
            [FromQuery] PatientFilter filter,
            int StartIndex,
            int Count,
            CancellationToken cancellationToken)
            => (await Sender.Send(new PatientGetAllQuery(PatientQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{patientId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid patientId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new PatientSoftDeleteCommand(patientId), cancellationToken)).ToActionResult();
    }
}
