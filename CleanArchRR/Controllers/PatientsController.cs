using Application.Entity.Patients.Commands.CreateCommand;
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
        [HttpPost("Create")]
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
        
    }
}
