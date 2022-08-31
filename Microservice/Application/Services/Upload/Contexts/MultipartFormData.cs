using Microservice.Application.Services.Upload.Contexts.Base;

namespace Microservice.Application.Services.Upload.Contexts
{
    //POCO Class used for DI! Multiple implementations of IUploadService
    public class MultipartFormData : UploadContext { }
}
