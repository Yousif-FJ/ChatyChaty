using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.FileRepository
{
    public class FileRepository : IFileRepository
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public FileRepository(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        public async Task<string> ChangePhoto(string PhotoName, IFormFile formFile)
        {
            var FilePath = Path.Combine(hostEnvironment.WebRootPath, "ProfilePIctures", PhotoName);
            using FileStream fileStream = new FileStream(path: FilePath, FileMode.Create);
            await formFile.CopyToAsync(fileStream);
            return PhotoName;
        }

        public async Task<string> UploadPhoto(IFormFile formFile)
        {
            var PhotoName = $"{Guid.NewGuid().ToString()}_{formFile.FileName}";
            var FilePath = Path.Combine(hostEnvironment.WebRootPath, "ProfilePIctures", PhotoName);
            using FileStream fileStream = new FileStream(path: FilePath, FileMode.CreateNew);
            await formFile.CopyToAsync(fileStream);
            return PhotoName;
        }
    }
}
