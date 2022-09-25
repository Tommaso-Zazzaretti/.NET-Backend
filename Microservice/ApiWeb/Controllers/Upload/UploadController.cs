using Microservice.ApiWeb.Filters;
using Microservice.Application.Services.Upload.Contexts;
using Microservice.Application.Services.Upload.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.ApiWeb.Controllers.Upload
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService<MultipartFormData> _uploadMultipartFormDataService;

        public UploadController(IUploadService<MultipartFormData> UploadService) {
            this._uploadMultipartFormDataService = UploadService;
        }

        [HttpPost("multipart-form-data")]
        [DisableRequestSizeLimit]
        [EnableHttpTrafficLimiter(MaxHttpRequestAllowedAtTheSameTime = 1)]
        public async Task<IActionResult> Upload() {  // For large files (size > 28 MB)
            await this._uploadMultipartFormDataService.UploadHandlerAsync();
            return Ok();
        }
    }
}
