using Microservice.Application.Services.Download.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.ApiWeb.Controllers.Download
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IDownloadService _downloadService;

        public DownloadController(IDownloadService DownloadService)
        {
            this._downloadService = DownloadService;
        }

        [HttpGet("file/{FileName}")]
        public IActionResult DownloadSingleFile(string FileName)
        {
            FileStreamResult FileResult = this._downloadService.GetSingleFileAsStream(FileName);
            FileResult.FileDownloadName = FileName;
            return FileResult;
        }

        [HttpGet("zip")]
        public async Task<IActionResult> DownloadMultipleFiles([FromBody]IEnumerable<string> FileNames)
        {
            FileStreamResult ZipFile = await this._downloadService.GetMultipleFilesAsZipStream(FileNames);
            ZipFile.FileDownloadName = "download.zip";
            return ZipFile;
        }
    }
}
