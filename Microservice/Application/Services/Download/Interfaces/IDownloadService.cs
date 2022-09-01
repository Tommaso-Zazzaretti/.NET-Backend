using Microsoft.AspNetCore.Mvc;

namespace Microservice.Application.Services.Download.Interfaces
{
    public interface IDownloadService
    {
        public FileStreamResult GetSingleFileAsStream(string FileName);
        public Task<FileStreamResult> GetMultipleFilesAsZipStream(IEnumerable<string> FileNames);
    }
}
