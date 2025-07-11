using Application.Entity.Doctors.Commands.Create;
using Application.Entity.Doctors.Commands.SoftDelete;
using Application.Entity.Doctors.Commands.Update;
using Application.Entity.Doctors.Queries;
using Application.Entity.Doctors.Queries.Get;
using Application.Entity.Doctors.Queries.GetAll;
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
    public class DoctorsController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] DoctorCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{doctorId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid doctorId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new DoctorSoftDeleteCommand(doctorId), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut]
        public async Task<IActionResult> Update(
            DoctorUpdateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] DoctorFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new DoctorGetAllQuery(DoctorQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet]
        public async Task<IActionResult> Take(
            [FromQuery] DoctorFilter filter,
            int StartIndex,
            int Count,
            CancellationToken cancellationToken)
            => (await Sender.Send(new DoctorGetAllQuery(DoctorQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{doctorId:guid}")]
        public async Task<IActionResult> Get(
            Guid doctorId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new DoctorGetQuery(DoctorQueries.GetById(doctorId)), cancellationToken)).ToActionResult();

    }
}
