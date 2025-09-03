using Application.Entity.HealthCondTypes.Commands.Create;
using Application.Entity.HealthCondTypes.Commands.SoftDelete;
using Application.Entity.HealthCondTypes.Commands.Update;
using Application.Entity.HealthCondTypes.Queries;
using Application.Entity.HealthCondTypes.Queries.Get;
using Application.Entity.HealthCondTypes.Queries.GetAll;
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
	public class HealthCondTypesController(ISender sender) : ApiController(sender)
	{
		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpPost]
		public async Task<IActionResult> Create(
			[FromBody] HealthCondTypeCreateCommand command,
			CancellationToken cancellationToken)
			=> (await Sender.Send(command, cancellationToken)).ToActionResult();
		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpPut]
		public async Task<IActionResult> Update(
			[FromBody] HealthCondTypeUpdateCommand command,
			CancellationToken cancellationToken)
			=> (await Sender.Send(command, cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpDelete("{healthCondTypeId:guid}")]
		public async Task<IActionResult> RemoveById(
			Guid healthCondTypeId,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new HealthCondTypeSoftDeleteCommand(healthCondTypeId), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("All")]
		public async Task<IActionResult> GetAll(
			[FromQuery] HealthCondTypeFilter filter,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new HealthCondTypeGetAllQuery(HealthCondTypeQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("Take")]
		public async Task<IActionResult> Take(
			[FromQuery] HealthCondTypeFilter filter,
			int StartIndex,
			int Count,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new HealthCondTypeGetAllQuery(HealthCondTypeQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("{healthCondTypeId:guid}")]
		public async Task<IActionResult> Get(
			Guid healthCondTypeId,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new HealthCondTypeGetQuery(HealthCondTypeQueries.GetById(healthCondTypeId)), cancellationToken)).ToActionResult();

	}
}