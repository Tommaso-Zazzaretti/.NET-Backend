using Microservice.Application.Services.Upload.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.ApiWeb.Controllers.Upload
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;

        public UploadController(IUploadService UploadService)
        {
            this._uploadService = UploadService;
        }

        [HttpPost("multipart/form-data")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload() {
            await this._uploadService.UploadMultipartRequestHandler();
            return Ok();
        }
    }
}
