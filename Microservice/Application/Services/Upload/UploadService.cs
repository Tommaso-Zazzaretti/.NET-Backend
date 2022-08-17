using Microservice.Application.Services.Upload.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Microservice.Application.Services.Upload
{
    public class UploadService : IUploadService
    {
        private readonly IHttpContextAccessor _httpCtxAccessor;
        private readonly ISet<String> _allowedFileExtensions;
        private readonly string _uploadLocationPath;
        private readonly int _maxBoundaryLength = 70;
        private readonly int _minBoundaryLength = 10;

        public UploadService(IHttpContextAccessor HttpAccessor, IHostEnvironment HostEnv, IConfiguration Configuration)
        {
            this._httpCtxAccessor = HttpAccessor;
            this._allowedFileExtensions = new HashSet<String>(Configuration.GetSection("Upload:AllowedFileExtensions").Get<string[]>());
            this._uploadLocationPath = Path.Combine(HostEnv.ContentRootPath, Configuration["Upload:LocalPath"]);
        }

        public async Task UploadMultipartRequestHandler()
        {
            var HttpRequest = this._httpCtxAccessor.HttpContext!.Request;
            //Check the Content Type (must be "multipart/form-data")
            if (string.IsNullOrWhiteSpace(HttpRequest.ContentType) || !HttpRequest.ContentType.Contains("multipart/form-data")) {
                throw new Exception("The multipart/form-data 'Content-Type' field is missing.");
            }
            //Check the Boundary
            MediaTypeHeaderValue ParsedContentType = MediaTypeHeaderValue.Parse(HttpRequest.ContentType);
            string? Boundary = HeaderUtilities.RemoveQuotes(ParsedContentType.Boundary).Value;
            if (string.IsNullOrWhiteSpace(Boundary))       { throw new Exception("Missing Boundary.")  ; }
            if (Boundary.Length < this._minBoundaryLength) { throw new Exception("Boundary too small."); }
            if (Boundary.Length > this._maxBoundaryLength) { throw new Exception("Boundary too large."); }
            //Check if the UploadDirectory exist
            if (!Directory.Exists(this._uploadLocationPath)) { 
                Directory.CreateDirectory(this._uploadLocationPath);
            }
            //Init a set containing all the Filenames created in this request
            ISet<string> CreatedFiles = new HashSet<string>();
            //Init the Http Multipart Stream Reader
            MultipartReader Reader = new MultipartReader(Boundary, HttpRequest.Body);
            try { 
                //Read the first Chunk
                MultipartSection? Chunk  = await Reader.ReadNextSectionAsync();
                while(Chunk != null) {
                    //Check the necessary ContentDispositionHeader (which contains File Metadata such as 'name' html form field)
                    if(string.IsNullOrWhiteSpace(Chunk.ContentDisposition)) {
                        throw new Exception("Missing ContentDisposition Header in a Chunk.");
                    }
                    ContentDispositionHeaderValue ParsedContentDisposition = ContentDispositionHeaderValue.Parse(Chunk.ContentDisposition);
                    string? FileName = ParsedContentDisposition.FileName.Value;
                    if (!ParsedContentDisposition.DispositionType.Equals("form-data") || string.IsNullOrWhiteSpace(FileName)) {
                        throw new Exception("The Upload Request cannot be processed. There are non-File fields.");
                    }
                    //Check the file extension
                    IEnumerable<string> SplittedFileName = FileName.Split(".");
                    if(!this._allowedFileExtensions.Contains(SplittedFileName.Last())) {
                        throw new Exception("The Upload Request cannot be processed. Forbidden file extension.");
                    }
                    //Check if the file already exist in the directory but it is not created during this request
                    string FilePath = Path.Combine(this._uploadLocationPath, FileName);
                    if (File.Exists(FilePath) && !CreatedFiles.Contains(FileName)) {
                        throw new Exception("The Upload Request cannot be processed. " + FileName + " already exist.");
                    }
                    //Check if the current chunk is a new File 
                    FileMode OpenMode = CreatedFiles.Contains(FileName) ? FileMode.Append : FileMode.Create;
                    if (OpenMode == FileMode.Create) { CreatedFiles.Add(FileName); }
                    //Write the current Chunk in a file
                    FileStream WriteStream = new FileStream(FilePath, OpenMode, FileAccess.Write, FileShare.None, 4096, true);
                    await Chunk.Body.CopyToAsync(WriteStream);
                    WriteStream.Close();
                    //Read the next section and so on..
                    Chunk = await Reader.ReadNextSectionAsync();
                }
            }
            catch (Exception Ex) {
                await this.RemoveFilesAsync(CreatedFiles);
                throw new Exception(Ex.Message);
            }
            return;
        }

        private async Task RemoveFilesAsync(IEnumerable<string> FileNames) {
            foreach(string FileName in FileNames) {
                string FilePath = Path.Combine(this._uploadLocationPath, FileName);
                if(File.Exists(FilePath)) {
                    await Task.Run(() => { File.Delete(FilePath); });
                }
            }
        }
    }
}
