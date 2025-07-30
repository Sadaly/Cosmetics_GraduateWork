using Application.Entity.Notifications.Commands.Create;
using Application.Entity.Notifications.Commands.SoftDelete;
using Application.Entity.Notifications.Commands.UpdateMessage;
using Application.Entity.Notifications.Commands.UpdatePhoneNumber;
using Application.Entity.Notifications.Queries;
using Application.Entity.Notifications.Queries.Get;
using Application.Entity.Notifications.Queries.GetAll;
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
    public class NotificationsController(ISender sender) : ApiController(sender)
    {
        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] NotificationCreateCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut("UpdateMessage")]
        public async Task<IActionResult> UpdateMessage(
            [FromBody] NotificationUpdateMessageCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpPut("UpdatePhonenumber")]
        public async Task<IActionResult> UpdatePhonenumber(
            [FromBody] NotificationUpdatePhoneNumberCommand command,
            CancellationToken cancellationToken)
            => (await Sender.Send(command, cancellationToken)).ToActionResult();


        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpDelete("{notificationId:guid}")]
        public async Task<IActionResult> RemoveById(
            Guid notificationId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new NotificationSoftDeleteCommand(notificationId), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("All")]
        public async Task<IActionResult> GetAll(
            [FromQuery] NotificationFilter filter,
            CancellationToken cancellationToken)
            => (await Sender.Send(new NotificationGetAllQuery(NotificationQueries.GetByFilter(filter)), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("Take")]
        public async Task<IActionResult> Take(
            [FromQuery] NotificationFilter filter,
            int StartIndex,
            int Count,
            CancellationToken cancellationToken)
            => (await Sender.Send(new NotificationGetAllQuery(NotificationQueries.GetByFilter(filter), StartIndex, Count), cancellationToken)).ToActionResult();

        [Authorize(Policy = AuthorizePolicy.UserOnly)]
        [HttpGet("{notificationId:guid}")]
        public async Task<IActionResult> Get(
            Guid notificationId,
            CancellationToken cancellationToken)
            => (await Sender.Send(new NotificationGetQuery(NotificationQueries.GetById(notificationId)), cancellationToken)).ToActionResult();

    }
}
