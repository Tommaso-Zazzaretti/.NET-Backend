using Microservice.Application.Services.Upload.Contexts.Base;

namespace Microservice.Application.Services.Upload.Interfaces
{
    public interface IUploadService<T> where T : UploadContext
    {
        public Task UploadHandlerAsync();
    }
}
