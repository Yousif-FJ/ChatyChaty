using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    /// <summary>
    /// A class that manage uploading and accessing Photos stored in Cloudinary
    /// </summary>
    public class CloudinaryPictureProvider : IPictureProvider
    {
        private readonly Cloudinary cloudinary;
        private const string PlaceHolderURL
            = "https://res.cloudinary.com/da5y8c0lx/image/upload/v1584120925/ChatyChaty/Placeholder_h5xlzr.jpg";
        private const string FileConatiner = "ChatyChaty";

        public CloudinaryPictureProvider(Cloudinary cloudinary)
        {
            this.cloudinary = cloudinary;
        }

        /// <summary>
        /// Add/Update a Photo and use (username and ID) to uniquely idetitify a user's Photo
        /// </summary>
        /// <remarks>The method Add new Picture or Overrade existing Photo with the same name</remarks>
        /// <param name="UserID"></param>
        /// <param name="UserName"></param>
        /// <param name="formFile"></param>
        /// <returns>The Uploaded Photo idetitifier</returns>
        public async Task<string> ChangePhoto(long UserID, string UserName, IFormFile formFile)
        {
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(name: formFile.FileName,stream: formFile.OpenReadStream()),
                PublicId = GetPublicId(UserID, UserName),
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

        /// <summary>
        /// Retrieve the user's PhotoURL using UserId and UserName
        /// </summary>
        /// <param name="UserID">User's ID which is part of the stored Photo idetitifier</param>
        /// <param name="UserName">UserName which is part of the stored Photo idetitifier</param>
        /// <returns>user's PhotoURL</returns>
        public async Task<string> GetPhotoURL(long UserID, string UserName)
        {
            var resourceResult = await cloudinary.GetResourceAsync(GetPublicId(UserID, UserName));
            if (resourceResult != null)
            {
                return resourceResult.Url;
            }
            return GetPlaceHolderURL();
        }

        /// <summary>
        /// Return the URL for Photo place holder
        /// </summary>
        /// <returns>URL for Photo place holder</returns>
        public string GetPlaceHolderURL()
        {
            return PlaceHolderURL;
        }

        /// <summary>
        /// Delete user's photo using UserId and UserName
        /// </summary>
        /// <param name="UserID">User's ID which is part of the stored Photo idetitifier</param>
        /// <param name="UserName">UserName which is part of the stored Photo idetitifier</param>
        public async Task DeletePhoto(long UserID, string UserName)
        {
            var Result = await cloudinary.DeleteResourcesAsync(publicIds: GetPublicId(UserID, UserName));
            if (Result.Error is null)
            {
                return;
            }
            else
            {
                throw new Exception($"An error occured in the action with this message : {Result.Error}");
            }
        }
        /// <summary>
        /// Get PublicId (Which is used by cloudinary) using UserId and UserName
        /// </summary>
        /// <remarks>PublicId is the unique idetitifier found in the URL of a photo, it is used by the Cloudinary</remarks>
        /// <param name="UserID">User's ID which is part of the stored Photo idetitifier</param>
        /// <param name="UserName">UserName which is part of the stored Photo idetitifier</param>
        /// <returns>PublicId</returns>
        private string GetPublicId(long UserID, string UserName)
        {
            return $"{FileConatiner}/{UserName}{UserID}";
        }
    }
}
