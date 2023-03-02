using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using Subscriptions.API.Application.Commands;
using Subscriptions.API.Application.DtoModels;
using Subscriptions.API.Application.Queries;
using Subscriptions.Domain.Specifications;
using System.Security.Claims;

namespace Subscriptions.API.Controllers {
    [ApiController, Route("api/v1/[controller]")]
    public class SubscriptionsController : ControllerBase {

        private readonly IMediator _mediator;

        public SubscriptionsController (IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost, Authorize(Policy = "user")]
        public async Task<IActionResult> Subscribe (SubscribeRequestDto request, CancellationToken cancellationToken) {
            await _mediator.Send(new SubscribeToUserCommand(GetUserId(), request.UserId, request.NotificationType), cancellationToken);
            return Ok();
        }

        [HttpPut, Authorize(Policy = "user")]
        public async Task<IActionResult> ChangeNotificationType (ChangeNotificationTypeRequestDto request, CancellationToken cancellationToken) {
            await _mediator.Send(new ChangeNotificationTypeCommand(GetUserId(), request.UserId, request.NotificationType), cancellationToken);
            return Ok();
        }

        [HttpDelete, Authorize(Policy = "user")]
        public async Task<IActionResult> Unsubscribe (string userId, CancellationToken cancellationToken) {
            await _mediator.Send(new UnsubscribeFromUserCommand(GetUserId(), userId), cancellationToken);
            return Ok();
        }

        [HttpGet("Detailed"), Authorize(Policy = "user")]
        public async Task<ActionResult<DetailedSubscriptionsDto>> GetDetailedSubscriptions ([FromQuery] GetSubscriptionsRequestDto request, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(
                new GetDetailedSubscriptionsQuery(
                    GetUserId(),
                    request.Sort ?? SubscriptionTargetSort.DisplayName,
                    new Pagination(request.Page ?? 1, request.PageSize ?? 50),
                    request.IncludeTotalCount ?? false
                ),
                cancellationToken));
        }


        [HttpGet, Authorize(Policy = "user")]
        public async Task<ActionResult<SubscriptionsDto>> GetSubscriptions ([FromQuery] GetSubscriptionsRequestDto request, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(
                new GetSubscriptionsQuery(
                    GetUserId(),
                    request.Sort ?? SubscriptionTargetSort.SubscriptionDate,
                    request.Page.HasValue && request.PageSize.HasValue ? new Pagination(request.Page.Value, request.PageSize.Value) : null,
                    request.IncludeTotalCount ?? false
                ),
                cancellationToken));
        }

        [HttpGet("Status")]
        public async Task<ActionResult<SubscriptionStatusDto>> GetSubscriptionStatus (string? subscriptionTargetId, string? subscriptionTargetHandle, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetSubscriptionStatusQuery(subscriptionTargetId, subscriptionTargetHandle, GetOptionalUserId()), cancellationToken));
        }

        [HttpGet("{userId}/Info")]
        public async Task<ActionResult<UserSubscriptionInfoDto>> GetUserSubscriptionInfo (string userId, CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetUserSubscriptionInfoQuery(userId), cancellationToken));
        }

        [HttpGet("Info"), Authorize(Policy = "User")]
        public async Task<ActionResult<UserSubscriptionInfoDto>> GetUserSubscriptionInfo (CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetUserSubscriptionInfoQuery(GetUserId()), cancellationToken));
        }

        [HttpGet("Ids"), Authorize(Policy = "user")]
        public async Task<ActionResult<SubscriptionTargetIdsDto>> GetSubscriptionTargetIds (CancellationToken cancellationToken) {
            return Ok(await _mediator.Send(new GetSubscriptionTargetIdsQuery(GetUserId()), cancellationToken));
        }

        private string? GetOptionalUserId () {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            return isAuthenticated ? HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
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
