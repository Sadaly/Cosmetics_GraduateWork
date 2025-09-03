using Application.Entity.SkinFeatures.Commands.ChangeType;
using Application.Entity.SkinFeatures.Commands.Create;
using Application.Entity.SkinFeatures.Commands.SoftDelete;
using Application.Entity.SkinFeatures.Queries;
using Application.Entity.SkinFeatures.Queries.Get;
using Application.Entity.SkinFeatures.Queries.GetAll;
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
	public class SkinFeaturesController(ISender sender) : ApiController(sender)
	{
		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpPost]
		public async Task<IActionResult> Create(
			[FromBody] SkinFeatureCreateCommand command,
			CancellationToken cancellationToken)
			=> (await Sender.Send(command, cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpPut]
		public async Task<IActionResult> ChangeType(
			[FromBody] SkinFeatureChangeTypeCommand command,
			CancellationToken cancellationToken)
			=> (await Sender.Send(command, cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpDelete("{skinFeatureId:guid}")]
		public async Task<IActionResult> RemoveById(
			Guid skinFeatureId,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new SkinFeatureSoftDeleteCommand(skinFeatureId), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("All")]
		public async Task<IActionResult> GetAll(
			[FromQuery] SkinFeatureFilter filter,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new SkinFeatureGetAllQuery(SkinFeatureQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("Take")]
		public async Task<IActionResult> Take(
			[FromQuery] SkinFeatureFilter filter,
			int StartIndex,
			int Count,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new SkinFeatureGetAllQuery(SkinFeatureQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();


		[Authorize(Policy = AuthorizePolicy.UserOnly)]
		[HttpGet("{skinFeatureId:guid}")]
		public async Task<IActionResult> Get(
			Guid skinFeatureId,
			CancellationToken cancellationToken)
			=> (await Sender.Send(new SkinFeatureGetQuery(SkinFeatureQueries.GetById(skinFeatureId)), cancellationToken)).ToActionResult();

	}
}
