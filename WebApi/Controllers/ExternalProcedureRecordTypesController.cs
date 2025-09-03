using Application.Entity.ExternalProcedureRecordTypes.Commands.Create;
using Application.Entity.ExternalProcedureRecordTypes.Commands.SoftDelete;
using Application.Entity.ExternalProcedureRecordTypes.Commands.Update;
using Application.Entity.ExternalProcedureRecordTypes.Queries;
using Application.Entity.ExternalProcedureRecordTypes.Queries.Get;
using Application.Entity.ExternalProcedureRecordTypes.Queries.GetAll;
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
	public class ExternalProcedureRecordTypesController(ISender sender) : ApiController(sender)
	{
		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpPost]
		public async Task<IActionResult> Create(
			[FromBody] ExternalProcedureRecordTypeCreateCommand command,
			CancellationToken cancellationToken)
			=> (await Sender.Send(command, cancellationToken)).ToActionResult();
		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpPut]
		public async Task<IActionResult> Update(
			[FromBody] ExternalProcedureRecordTypeUpdateCommand command,
			CancellationToken cancellationToken)
			=> (await Sender.Send(command, cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpDelete("{externalProcedureRecordTypeId:guid}")]
		public async Task<IActionResult> RemoveById(
			Guid externalProcedureRecordTypeId,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new ExternalProcedureRecordTypeSoftDeleteCommand(externalProcedureRecordTypeId), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("All")]
		public async Task<IActionResult> GetAll(
			[FromQuery] ExternalProcedureRecordTypeFilter filter,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new ExternalProcedureRecordTypeGetAllQuery(ExternalProcedureRecordTypeQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("Take")]
		public async Task<IActionResult> Take(
			[FromQuery] ExternalProcedureRecordTypeFilter filter,
			int StartIndex,
			int Count,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new ExternalProcedureRecordTypeGetAllQuery(ExternalProcedureRecordTypeQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("{externalProcedureRecordTypeId:guid}")]
		public async Task<IActionResult> Get(
			Guid externalProcedureRecordTypeId,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new ExternalProcedureRecordTypeGetQuery(ExternalProcedureRecordTypeQueries.GetById(externalProcedureRecordTypeId)), cancellationToken)).ToActionResult();

	}
}