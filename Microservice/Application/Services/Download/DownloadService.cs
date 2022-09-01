using Microservice.Application.Services.Download.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace Microservice.Application.Services.Download
{
    public class DownloadService : IDownloadService
    {
        private readonly IDictionary<string, string> _MIMEMapping = new Dictionary<string, string>() {
            { "txt" , "text/plain" },
            { "zip" , "application/zip" },
            { "json", "application/json" }
        };
        private readonly string _fileLocationPath;

        public DownloadService(IHostEnvironment HostEnv, IConfiguration Configuration) {
            this._fileLocationPath = Path.Combine(HostEnv.ContentRootPath, Configuration["Upload:LocalPhysicalPath"]);
        }

        public FileStreamResult GetSingleFileAsStream(string FileName)
        {
            //MIME Type check
            string FileExtension = FileName.Split(".").Last();
            if (!this._MIMEMapping.ContainsKey(FileExtension)) {
                throw new Exception("The Download Request cannot be processed. Cannot find MIME type of '."+FileExtension+"'");
            }
            //Check if the UploadDirectory exist
            if (!Directory.Exists(this._fileLocationPath)) {
                throw new Exception("The Download Request cannot be processed. Cannot find Files location");
            }
            //Check if the Requested file exist 
            string FilePath = Path.Combine(this._fileLocationPath, FileName);
            if (!File.Exists(FilePath)) {
                throw new Exception("The Download Request cannot be processed. " + FileName + " does not exist.");
            }
            //Return a FileStreamResult
            FileStream FileStreamReader = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,true);
            string MIMEType = this._MIMEMapping[FileExtension];
            return new FileStreamResult(FileStreamReader, MIMEType);
        }

        public async Task<FileStreamResult> GetMultipleFilesAsZipStream(IEnumerable<string> FileNames)
        {
            //Check if the UploadDirectory exist
            if (!Directory.Exists(this._fileLocationPath)) {
                throw new Exception("The Download Request cannot be processed. Cannot find Files location");
            }
            string ZipName = Guid.NewGuid().ToString() + "download.zip"; //Generate a Guid for each request in order to univocate the .zip file names
            string ZipPath = Path.Combine(this._fileLocationPath, ZipName);
            //Build a temporary Zip File in Async mode
            FileStream ZipStreamWriter = new FileStream(ZipPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            ZipArchive Archive = new ZipArchive(ZipStreamWriter, ZipArchiveMode.Create);
            try {
                foreach (string FileName in FileNames) {
                    await Task.Run(() => {
                        //Check if the current file exist 
                        //Console.WriteLine(Environment.CurrentManagedThreadId);
                        string FilePath = Path.Combine(this._fileLocationPath, FileName);
                        if (!File.Exists(FilePath)) {
                            Archive.Dispose(); ZipStreamWriter.Close();
                            throw new Exception("The Download Request cannot be processed. " + FileName + " does not exist.");
                        }
                        Archive.CreateEntryFromFile(FilePath, FileName, CompressionLevel.Optimal);
                    });
                }
                Archive.Dispose(); 
                ZipStreamWriter.Close();
                /*
                    Return a FileStreamResult. 
                    The Stream object passed to a FileStreamResult is automatically closed by his method:
                            => protected override void WriteFile(HttpResponseBase response)
                    which is invoked invoked during the transmission of the http response.
                    I set the FileOptions.DeleteOnClose option in order to delete the temporary Zip file after the streaming and clean up the directory
                    So, when the controller processes the response, the stream will be closed and the file will be deleted.  
                */
                FileStream ZipStreamReader = new FileStream(ZipPath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);
                return new FileStreamResult(ZipStreamReader, "application/zip");
            } 
            catch (Exception ex){
                if (File.Exists(ZipPath)) {
                    await Task.Run(() => { File.Delete(ZipPath); });
                }
                throw new Exception(ex.Message);
            };
        }
    }
}
