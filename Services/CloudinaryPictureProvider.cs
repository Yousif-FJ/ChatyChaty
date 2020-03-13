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
        public async Task<string> ChangePhoto(string PhotoID, IFormFile formFile)
        {
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(name: formFile.FileName,stream: formFile.OpenReadStream()),
                PublicId = PhotoID,
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

        public async Task<string> GetPhotoURL(string PhotoID)
        {
            var resourceResult = await cloudinary.GetResourceAsync(PhotoID);
            return resourceResult.Url;
        }

        public async Task<string> UploadPhoto(IFormFile formFile)
        {
            var giud = Guid.NewGuid().ToString();
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(name: formFile.FileName, stream: formFile.OpenReadStream()),
                PublicId = $"ChatyChaty/{giud}",
                Overwrite = false,
                
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

        public string GetPlaceHolderURL()
        {
            return PlaceHolderURL;
        }
    }
}
