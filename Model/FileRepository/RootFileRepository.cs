using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.FileRepository
{
    public class RootFileRepository : IFileRepository
    {
        private readonly string fileConatiner;

        public string FileConatiner
        {
            get
            {
                Directory.CreateDirectory(fileConatiner);
                return fileConatiner;
            }
        }

        public RootFileRepository(IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment is null) { throw new ArgumentNullException(nameof(webHostEnvironment)); };
            fileConatiner = Path.Combine(webHostEnvironment.WebRootPath, "ProfilePIctures");
        }

        public async Task<string> ChangePhoto(string PhotoName, IFormFile formFile)
        {
            var FilePath = Path.Combine(fileConatiner, PhotoName);
            using FileStream fileStream = new FileStream(path: FilePath, FileMode.Create);
            await formFile.CopyToAsync(fileStream);
            return PhotoName;
        }

        public async Task<string> UploadPhoto(IFormFile formFile)
        {
            var PhotoName = $"{Guid.NewGuid().ToString()}_{formFile.FileName}";
            var FilePath = Path.Combine(fileConatiner, PhotoName);
            using FileStream fileStream = new FileStream(path: FilePath, FileMode.CreateNew);
            await formFile.CopyToAsync(fileStream);
            return PhotoName;
        }
    }
}
