using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using FileStorage.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;

namespace MyFileManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly MyAttachmentsContext _context;
        public HomeController(IConfiguration configuration, MyAttachmentsContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        // GET: api/Home
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string connectionString = _configuration["Azure:Storage:ConnectionString"];

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("f5500");

            if (blobContainerClient != null)
                return Ok("Hello I'm Good!! :" + blobContainerClient.AccountName);
            else
                return BadRequest();
        }

        // POST: api/Home
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] IFormFile file)
        {
            string connectionString = _configuration["Azure:Storage:ConnectionString"];

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("f5500");

            BlobClient blobClient = blobContainerClient.GetBlobClient(file.FileName);

            var memory = new MemoryStream();
            await file.CopyToAsync(memory);
            memory.Position = 0;

            var result = await blobClient.UploadAsync(memory, true);
            memory.Close();

            AttachmentDocument document = new AttachmentDocument()
            {
                DateCreated = DateTime.Now,
                FileName = file.FileName,
                DocPath = blobClient.Uri.AbsoluteUri
            };

            _context.AttachmentDocument.Add(document);
            _context.SaveChanges();


            return Ok(document.Id);

        }

        // GET: api/Home/5
        [HttpGet("Download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var document = _context.AttachmentDocument.Find(id);

            string connectionString = _configuration["Azure:Storage:ConnectionString"];

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("f5500");

            BlobClient blobClient = blobContainerClient.GetBlobClient(document.FileName);

            var memory = new MemoryStream();
            var download = blobClient.DownloadTo(memory);
            memory.Position = 0;

            return new FileStreamResult(memory, GetContentType(document.DocPath))
            {
                FileDownloadName = Path.GetFileName(document.DocPath)
            };
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            


        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
