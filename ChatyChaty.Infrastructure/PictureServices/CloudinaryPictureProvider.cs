using ChatyChaty.Domain.ApplicationExceptions;
using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
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
        private const string baseUrl = "https://res.cloudinary.com/da5y8c0lx/image/upload/";
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
        public async Task<string> ChangePhoto(UserId userID, string fileName, Stream File)
        {
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(name: fileName, stream: File),
                PublicId = ConstructPublicId(userID),
                Overwrite = true
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error is null)
            {
                return uploadResult.SecureUrl.AbsoluteUri;
            }
            else
            {
                throw new PictureProviderException(uploadResult.Error.Message);
            }
        }

        /// <summary>
        /// Delete user's photo using UserId and UserName
        /// </summary>
        /// <param name="UserID">User's ID which is part of the stored Photo idetitifier</param>
        public async Task DeletePhoto(UserId userID)
        {
            var Result = await cloudinary.DeleteResourcesAsync(publicIds: ConstructPublicId(userID));
            if (Result.Error is null)
            {
                return;
            }
            else
            {
                throw new PictureProviderException($"An error occured in the action with this message : {Result.Error}");
            }
        }
        /// <summary>
        /// Get PublicId (Which is used by cloudinary) using UserId and UserName
        /// </summary>
        /// <remarks>PublicId is the unique idetitifier found in the URL of a photo, it is used by the Cloudinary</remarks>
        /// <param name="UserID">User's ID which is part of the stored Photo idetitifier</param>
        private static string ConstructPublicId(UserId UserID)
        {
            return $"{FileConatiner}/{UserID}";
        }
    }
}
