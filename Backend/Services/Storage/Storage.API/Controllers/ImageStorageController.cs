using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Application.Commands;
using Storage.API.Application.DtoModels;
using Storage.API.Utilities;
using System.Security.Claims;

namespace Storage.Controllers {
    [ApiController, Route("api/v1/[controller]")]
    public class ImageStorageController : ControllerBase {

        private readonly IMediator _mediator;

        public ImageStorageController (IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost, Authorize(Policy = "user")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<ImageUploadResponseDto>> UserUploadImage () {
            var request = HttpContext.Request;

            var token = request.Query["Token"].ToString();
            var imageSection = await request.FindMultipartSection("file");

            if (token == null || imageSection == null) {
                return BadRequest();
            }

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) {
                return Unauthorized();
            }

            return Ok(await _mediator.Send(new UserUploadImageCommand(userId, token, imageSection)));
        }

        [HttpPost("Internal"), Authorize(Policy = "service")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult<string>> ServiceUploadImage () {
            var request = HttpContext.Request;

            Guid fileId = Guid.Parse(request.Headers["FileId"].ToString());
            Guid groupId = Guid.Parse(request.Headers["GroupId"].ToString());
            string category = request.Headers["Category"].ToString();

            var imageSection = await request.FindMultipartSection("file");

            if (imageSection == null) {
                return BadRequest();
            }

            var storedFile = await _mediator.Send(new ServiceUploadImageCommand(fileId, groupId, category, imageSection));

            return Ok(storedFile.Url);
        }

    }
}
