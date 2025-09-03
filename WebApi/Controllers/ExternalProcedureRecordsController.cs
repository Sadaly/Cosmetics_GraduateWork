using Application.Entity.ExternalProcedureRecords.Commands.ChangeType;
using Application.Entity.ExternalProcedureRecords.Commands.Create;
using Application.Entity.ExternalProcedureRecords.Commands.SoftDelete;
using Application.Entity.ExternalProcedureRecords.Queries;
using Application.Entity.ExternalProcedureRecords.Queries.Get;
using Application.Entity.ExternalProcedureRecords.Queries.GetAll;
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
	public class ExternalProcedureRecordsController(ISender sender) : ApiController(sender)
	{
		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpPost]
		public async Task<IActionResult> Create(
			[FromBody] ExternalProcedureRecordCreateCommand command,
			CancellationToken cancellationToken)
			=> (await Sender.Send(command, cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpPut]
		public async Task<IActionResult> ChangeType(
			[FromBody] ExternalProcedureRecordChangeTypeCommand command,
			CancellationToken cancellationToken)
			=> (await Sender.Send(command, cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpDelete("{externalProcedureRecordId:guid}")]
		public async Task<IActionResult> RemoveById(
			Guid externalProcedureRecordId,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new ExternalProcedureRecordSoftDeleteCommand(externalProcedureRecordId), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("All")]
		public async Task<IActionResult> GetAll(
			[FromQuery] ExternalProcedureRecordFilter filter,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new ExternalProcedureRecordGetAllQuery(ExternalProcedureRecordQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("Take")]
		public async Task<IActionResult> Take(
			[FromQuery] ExternalProcedureRecordFilter filter,
			int StartIndex,
			int Count,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new ExternalProcedureRecordGetAllQuery(ExternalProcedureRecordQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("{externalProcedureRecordId:guid}")]
		public async Task<IActionResult> Get(
			Guid externalProcedureRecordId,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new ExternalProcedureRecordGetQuery(ExternalProcedureRecordQueries.GetById(externalProcedureRecordId)), cancellationToken)).ToActionResult();

	}
}
