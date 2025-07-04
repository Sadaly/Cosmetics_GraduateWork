﻿using Application.Entity.Patients.Commands.Create;
using Application.Entity.Patients.Commands.SoftDelete;
using Application.Entity.Patients.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;
using WebApi.Extensions;
using WebApi.Policies;
using WebApi.SupportData.Filters;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class PatientsController : ApiController
    {
        public PatientsController(ISender sender) : base(sender)
        {
        }

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] PatientCreateCommand command,
            CancellationToken cancellationToken)
            =>  (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] PatientFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new PatientsGetAllQuery(filter.ToPredicate()), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{patientId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid patientId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new PatientSoftDeleteCommand(patientId), cancellationToken)).ToActionResult();
    }
}
