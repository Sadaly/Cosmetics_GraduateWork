using Application.Entity.PatientCards.Commands.Update;
using Application.Entity.PatientCards.Queries;
using Application.Entity.PatientCards.Queries.Get;
using Application.Entity.PatientCards.Queries.GetAll;
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
	public class PatientCardsController(ISender sender) : ApiController(sender)
	{
		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpPut]
		public async Task<IActionResult> Update(
			[FromBody] PatientCardUpdateCommand command,
			CancellationToken cancellationToken)
			=> (await Sender.Send(command, cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("All")]
		public async Task<IActionResult> GetAll(
			[FromQuery] PatientCardFilter filter,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new PatientCardGetAllQuery(PatientCardQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("Take")]
		public async Task<IActionResult> Take(
			[FromQuery] PatientCardFilter filter,
			int StartIndex,
			int Count,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new PatientCardGetAllQuery(PatientCardQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("{patientcardId:guid}")]
		public async Task<IActionResult> Get(
			Guid patientcardId,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new PatientCardGetQuery(PatientCardQueries.GetById(patientcardId)), cancellationToken)).ToActionResult();

	}
}
