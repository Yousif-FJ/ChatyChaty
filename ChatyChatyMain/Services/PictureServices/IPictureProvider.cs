using ChatyChaty.Model.AccountModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services.PictureServices
{
    /// <summary>
    /// A class that manage uploading and accessing Photos stored in a Provider
    /// </summary>
    public interface IPictureProvider
    {
        public string GetPlaceHolderURL();
        public Task<PhotoUrlModel> ChangePhoto(long UserID,string UserName, IFormFile formFile);
        public Task<string> GetPhotoURL(long UserID,string UserName);
        public Task DeletePhoto(long UserID, string UserName);
    }
}
