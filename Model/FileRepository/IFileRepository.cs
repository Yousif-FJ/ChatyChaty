using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.FileRepository
{
    public interface IFileRepository
    {
        public Task<string> UploadPhoto(IFormFile formFile);
        public Task<string> ChangePhoto(string PhotoName, IFormFile formFile);
    }
}
