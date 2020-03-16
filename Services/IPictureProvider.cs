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
        public Task<string> ChangePhoto(long UserID, IFormFile formFile);
        public Task<string> GetPhotoURL(long UserID);
    }
}
