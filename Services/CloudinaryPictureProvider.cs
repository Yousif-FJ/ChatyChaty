using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public class CloudinaryPictureProvider : IPictureProvider
    {
        private readonly Cloudinary cloudinary;
        private const string PlaceHolderURL
            = "https://res.cloudinary.com/da5y8c0lx/image/upload/v1584120925/ChatyChaty/Placeholder_h5xlzr.jpg";

        public CloudinaryPictureProvider(Cloudinary cloudinary)
        {
            this.cloudinary = cloudinary;
        }
        public async Task<string> ChangePhoto(long UserID, string UserName, IFormFile formFile)
        {
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(name: formFile.FileName,stream: formFile.OpenReadStream()),
                PublicId = $"ChatyChaty/{UserName}{UserID}",
                Overwrite = true
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error is null)
            {
                return uploadResult.PublicId;
            }
            else
            {
                throw new Exception(uploadResult.Error.Message);
            }
        }

        public async Task<string> GetPhotoURL(long UserID, string UserName)
        {
            var resourceResult = await cloudinary.GetResourceAsync(publicId : $"ChatyChaty/{UserName}{UserID}");
            return resourceResult.Url;
        }

        public string GetPlaceHolderURL()
        {
            return PlaceHolderURL;
        }
    }
}
