using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using Subscriptions.API.Application.Commands;
using Subscriptions.API.Application.DtoModels;
using Subscriptions.API.Application.Queries;
using System.Security.Claims;

namespace Subscriptions.API.Controllers {
    [ApiController, Route("api/v1/[controller]"), Authorize(Policy = "user")]
    public class NotificationsController : ControllerBase {

        private readonly IMediator _mediator;

        public NotificationsController (IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet("Count")]
        public async Task<ActionResult<int>> GetNotificationMessageCount (CancellationToken cancellationToken = default) {
            return Ok(await _mediator.Send(new GetNotificationMessageCountQuery(GetUserId()), cancellationToken));
        }

        [HttpGet("UnreadCount")]
        public async Task<ActionResult<int>> GetUnreadNotificationMessageCount (CancellationToken cancellationToken = default) {
            return Ok(await _mediator.Send(new GetUnreadNotificationMessageCountQuery(GetUserId()), cancellationToken));
        }

        [HttpPut("UnreadCount")]
        public async Task<IActionResult> ResetUnreadNotificationMessageCount (CancellationToken cancellationToken = default) {
            await _mediator.Send(new ResetUnreadNotificationMessageCountCommand(GetUserId()), cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<GetNotificationMessageResponseDto>> GetNotificationMessages (int? page = null, int? pageSize = null, CancellationToken cancellationToken = default) {
            return Ok(await _mediator.Send(new GetNotificationMessagesQuery(GetUserId(), page, pageSize), cancellationToken));
        }

        [HttpPut("{messageId}")]
        public async Task<IActionResult> MarkNotificationMessageChecked (string messageId, CancellationToken cancellationToken = default) {
            return Ok(await _mediator.Send(new MarkNotificationMessageCheckedCommand(GetUserId(), messageId), cancellationToken));
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> RemoveNotificationMessage (string messageId, CancellationToken cancellationToken = default) {
            await _mediator.Send(new RemoveNotificationMessageFromUserCommand(GetUserId(), messageId), cancellationToken);
            return Ok();
        }

        private string GetUserId () {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) {
                throw new AppException("Unauthorized", null, StatusCodes.Status401Unauthorized);
            }
            return userId;
        }

    }
}
