using GymSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Classes
{
    public class AttachementServices : IAttachementServices
    {
        private readonly long maxFileSize = 5 * 1024 * 1024;
        private readonly string[] allowedExtentions = { ".jpg",".jpeg",".png" };
        private readonly ILogger<AttachementServices> logger;
        private readonly IWebHostEnvironment env;

        public AttachementServices(ILogger<AttachementServices> logger,IWebHostEnvironment env)
        {
            this.logger = logger;
            this.env = env;
        }

        public bool Delete(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName)) return false;

            try
            {
                var fullPath = Path.Combine(env.ContentRootPath, folderName, fileName);
                if (!File.Exists(fullPath)) return false;

                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed To Delete The Attachement");
                return false;
            }
        }

        public (Stream stream, string contentType)? GetFile(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName)) return null;
            var fullPath = Path.Combine(env.ContentRootPath,folderName, fileName);  
            if(!File.Exists(fullPath)) return null;

            var stream = new FileStream(fullPath,FileMode.Open,FileAccess.Read);

            var extension = Path.GetExtension(fullPath).ToLower();

            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };
            return (stream, contentType);

        }



        public async Task<string?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream is null || !fileStream.CanRead) return null;

            if (fileStream.Length > maxFileSize)
            {
                logger.LogWarning("Rejected File Too Large");
                return null;

            }
            var extention = Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extention) || !allowedExtentions.Contains(extention))
            {
                logger.LogWarning("Reject Wrong Extention File");
                return null;
            }
            var uploadedFolder = Path.Combine(env.ContentRootPath, folderName);

            Directory.CreateDirectory(uploadedFolder);
            var storedFileName = $"{Guid.NewGuid()}{extention}";
            var filePath = Path.Combine(uploadedFolder, storedFileName);
            try
            {
                await using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                await fileStream.CopyToAsync(fs);
                return storedFileName;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to upload file");
                return null;
            }
        }
    }
}
