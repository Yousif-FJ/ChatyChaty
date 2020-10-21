using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.AccountModel;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Infrastructure.PictureServices
{
    /// <summary>
    /// A class that manage uploading and accessing Photos stored in Cloudinary
    /// </summary>
    public class CloudinaryPictureProvider : IPictureProvider
    {
        private readonly Cloudinary cloudinary;
        private readonly ILogger<CloudinaryPictureProvider> logger;
        private const string PlaceHolderURL
            = "https://res.cloudinary.com/da5y8c0lx/image/upload/ChatyChaty/Placeholder_h5xlzr.jpg";
        private const string FileConatiner = "ChatyChaty";

        public CloudinaryPictureProvider(Cloudinary cloudinary, ILogger<CloudinaryPictureProvider> logger)
        {
            this.cloudinary = cloudinary;
            this.logger = logger;
        }

        /// <summary>
        /// Add/Update a Photo and use (username and ID) to uniquely idetitify a user's Photo
        /// </summary>
        /// <remarks>The method Add new Picture or Overrade existing Photo with the same name</remarks>
        /// <returns>The Uploaded Photo idetitifier</returns>
        public async Task<PhotoUrlModel> ChangePhoto(long UserID, string UserName, string fileName, Stream File)
        {
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(name: fileName, stream: File),
                PublicId = GetPublicId(UserID, UserName),
                Overwrite = true
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error is null)
            {
                return new PhotoUrlModel
                {
                    Success = true,
                    URL = uploadResult.SecureUrl.AbsoluteUri
                };
            }
            else
            {
                logger.LogError($"Img upload error : {uploadResult.Error.Message}");
                return new PhotoUrlModel
                {
                    Success = false,
                    Errors = new Collection<string>() { "An error occurred in the server" }
                };
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
            if (resourceResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return resourceResult.SecureUrl;
            }
            return null;
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
