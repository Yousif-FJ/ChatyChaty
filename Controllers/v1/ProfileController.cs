﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerSchema.v1;
using ChatyChaty.Model.FileRepository;
using ChatyChaty.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.ValidationAttributes;

namespace ChatyChaty.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IAccountManager accountManager;
        private readonly IPictureProvider pictureProvider;

        public ProfileController(IAccountManager accountManager, IPictureProvider pictureProvider)
        {
            this.accountManager = accountManager;
            this.pictureProvider = pictureProvider;
        }

        /// <summary>
        /// Takes a UserName and gives a Photo Location
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        /// <response code="404">The given UserName doesn't exist</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("GetUserPhoto")]
        public async Task<IActionResult> GetUserPhoto([FromQuery]string UserName)
        {
            var User = await accountManager.GetUser(UserName);
            if (User is null)
            {
                return NotFound("UserName wasn't found");
            }
            if (User.PhotoID != null)
            {
                return Ok(await pictureProvider.GetPhotoURL(User.PhotoID));
            }
            else
            {
                return Ok(pictureProvider.GetPlaceHolderURL());
            }
        }

        /// <summary>
        /// set photo picture or replace existing one (Require authentication)
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        [Authorize]
        [Consumes("multipart/form-data")]
        [HttpPost("SetPhotoForSelf")]
        public async Task<IActionResult> SetPhotoForSelf([FromForm]UploadFileSchema uploadFile)
        {         
            var UserNameClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            var user = await accountManager.GetUser(UserNameClaim.Value);
            string PhotoID;
            if (user.PhotoID is null)
            {
                PhotoID = await pictureProvider.UploadPhoto(uploadFile.PhotoFile);
                await accountManager.SetPhotoID(user.UserName, PhotoID);
            }
            else
            {
                PhotoID = await pictureProvider.ChangePhoto(user.PhotoID, uploadFile.PhotoFile);
            }
            var ReturnURL = await pictureProvider.GetPhotoURL(PhotoID);
                return Ok(ReturnURL);
        }
    }
}