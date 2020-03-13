using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public interface IPictureProvider
    {
        public string GetPlaceHolderURL();
        public Task<string> UploadPhoto(IFormFile formFile);
        public Task<string> ChangePhoto(string PhotoID, IFormFile formFile);
        public Task<string> GetPhotoURL(string PhotoID);
    }
}
