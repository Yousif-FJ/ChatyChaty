﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerSchema.v1;
using ChatyChaty.ControllerSchema.v1.Profile;
using ChatyChaty.Services;
using ChatyChaty.ValidationAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.ValidationAttributes;

namespace ChatyChaty.Controllers.v1
{
    [RequireHttpsOrClose]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IAccountManager accountManager;
        private readonly IPictureProvider pictureProvider;
        private readonly IMessageService messageService;

        public ProfileController(IAccountManager accountManager, IPictureProvider pictureProvider, IMessageService messageService)
        {
            this.accountManager = accountManager;
            this.pictureProvider = pictureProvider;
            this.messageService = messageService;
        }

        /// <summary>
        /// [This is no longer needed] Takes a UserName and gives a Photo Location
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        /// <response code="404">The given UserName doesn't exist</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("GetUserPhoto")]
        [Obsolete("This is no longer needed")]
        public async Task<IActionResult> GetUserPhoto([FromQuery]string UserName)
        {
            var User = await accountManager.GetUser(UserName);
            if (User is null)
            {
                return NotFound("UserName wasn't found");
            }
            var PictureURL = await pictureProvider.GetPhotoURL(User.Id,User.UserName);
            return Ok(PictureURL);
        }

        /// <summary>
        /// Set photo picture or replace existing one (Require authentication)
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        /// <response code="400">The uploaded Photo must be a vaild img with png, jpg or jpeg</response>
        /// <response code="401">Unaithenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [Consumes("multipart/form-data")]
        [HttpPost("SetPhotoForSelf")]
        public async Task<IActionResult> SetPhotoForSelf([FromForm]UploadFileSchema uploadFile)
        {         
            var UserNameClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            var user = await accountManager.GetUser(UserNameClaim.Value);
            await pictureProvider.ChangePhoto(user.Id,user.UserName, uploadFile.PhotoFile);

            var ReturnURL = await pictureProvider.GetPhotoURL(user.Id,user.UserName);
                return Ok(ReturnURL);
        }


        /// <summary>
        /// Find users and return user profile with a conversation Id which is used to send messages
        /// </summary>
        /// <remarks>This is used to start a conversation with a user</remarks>
        /// <param name="UserName"></param>
        /// <returns></returns>
        /// <response code="401">Unaithenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromQuery]string UserName)
        {
            var UserIdClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var user = await accountManager.GetUser(UserName);
            if (user == null)
            {
                return Ok(
                    new GetUserProfileResponse
                    {
                        Success = false,
                        Error = "No such a Username"
                    });
            }
            var conversationId = await messageService.NewConversation(long.Parse(UserIdClaim.Value), user.Id);
            var response = new GetUserProfileResponse
            {
                Success = true,
                ConversationId = conversationId,
                DisplayName = user.DisplayName,
                Username = user.UserName,
                PictureUrl = await pictureProvider.GetPhotoURL(user.Id, user.UserName)
            };
            return Ok(response);
        }


        /// <summary>
        /// Get conversation information
        /// </summary>
        /// <remarks>This is used when GetNewMessages return a message with a conversation Id that doesn't exist locally 
        /// i.e the user received a message first time from some other user</remarks>
        /// <param name="ConversationId"></param>
        /// <returns></returns>
        /// <response code="400">Requested conversationId doesn't exist</response>
        /// <response code="401">Unaithenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpGet("GetConversation")]
        public async Task<IActionResult> GetConversationInfo([FromQuery]long ConversationId)
        {
            var UserIdClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var conversation = await messageService.GetConversationInfo(long.Parse(UserIdClaim.Value), ConversationId);
            if (conversation == null)
            {
                return BadRequest();
            }
            var user = await accountManager.GetUser(UserIdClaim.Value);
            var response = new GetConversationInfoResponse
            {
                ConversationId = conversation.Id,
                DisplayName = user.DisplayName,
                Username = user.UserName,
                PictureUrl = await pictureProvider.GetPhotoURL(user.Id, user.UserName)
            };
            return Ok(response);
        }
    }
}