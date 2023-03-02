using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Application.Commands;
using Storage.API.Utilities;
using System.Security.Claims;

namespace Storage.Controllers {
    [ApiController, Route("api/v1/[controller]")]
    public class VideoStorageController : ControllerBase {

        private readonly IMediator _mediator;

        public VideoStorageController (IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost, Authorize(Policy = "user")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UserUploadVideo () {
            var request = HttpContext.Request;

            var token = request.Query["Token"].ToString();
            var videoSection = await request.FindMultipartSection("file");

            if (token == null || videoSection == null) {
                return BadRequest();
            }

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) {
                return Unauthorized();
            }

            await _mediator.Send(new UserUploadVideoCommand(userId, token, videoSection));

            return Ok();
        }

        [HttpPost("Internal"), Authorize(Policy = "service")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<string>> ServiceUploadVideo () {
            var request = HttpContext.Request;

            Guid fileId = Guid.Parse(request.Headers["FileId"].ToString());
            Guid groupId = Guid.Parse(request.Headers["GroupId"].ToString());
            string category = request.Headers["Category"].ToString();

            var videoSection = await request.FindMultipartSection("file");

            if (videoSection == null) {
                return BadRequest();
            }

            var storedFile = await _mediator.Send(new ServiceUploadVideoCommand(fileId, groupId, category, videoSection));

            return Ok(storedFile.Url);
        }

    }
}
