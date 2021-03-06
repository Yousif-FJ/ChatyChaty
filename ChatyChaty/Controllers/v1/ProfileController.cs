﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.HttpShemas.v1.Profile;
using ChatyChaty.HttpShemas.v1.Error;
using ChatyChaty.Domain.ApplicationExceptions;

namespace ChatyChaty.Controllers.v1
{
    [Authorize]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IAccountManager accountManager;

        public ProfileController(IAccountManager accountManager)
        {
            this.accountManager = accountManager;
        }


        /// <summary>
        /// Set photo or replace existing one (Require authentication)
        /// </summary>
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [HttpPost("Photo")]
        public async Task<IActionResult> SetPhotoForSelf([FromForm] UploadFileSchema uploadFile)
        {
            var userId = HttpContext.GetUserIdFromHeader();

            var user = await accountManager.SetPhotoAsync(
                userId, 
                uploadFile.PhotoFile.FileName,
                uploadFile.PhotoFile.OpenReadStream());

            return Ok(new ProfileResponse(user.UserName,user.DisplayName,user.PhotoURL));
        }


        /// <summary>
        /// Find users and return user profile with a chat Id,
        /// which is used to send messages and get other users info(Require authentication)
        /// </summary>
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [HttpGet("User")]
        public async Task<IActionResult> GetUser([FromQuery][Required] string userName)
        {
            var userId = HttpContext.GetUserIdFromHeader();
            try
            {
                var result = await accountManager.CreateConversationAsync(userId, userName);
                var response = new UserProfileResponse(result.ChatId.Value,
                                new ProfileResponse(result.Username, result.DisplayName,result.PhotoURL));

                return Ok(response);
            }
            catch (UserNotFoundException e)
            {
                return NotFound(new ErrorResponse(e.Message));
            }
        }

        /// <summary>
        /// Get a list of all chat's information (Require authentication)
        /// </summary>
        [ProducesResponseType(typeof(List<UserProfileResponse>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [HttpGet("Chats")]
        public async Task<IActionResult> GetChats()
        {
            var userId = HttpContext.GetUserIdFromHeader();

            var chats = await accountManager.GetConversations(userId);

            var chatListResponse = new List<UserProfileResponse>();

            foreach (var chat in chats)
            {
                chatListResponse.Add(
                new UserProfileResponse(chat.ChatId.Value,
                    new ProfileResponse(chat.Username, chat.DisplayName, chat.PhotoURL
                    )));
            };

            return Ok(chatListResponse);
        }


        /// <summary>
        /// Set or update the DisplayName of the authenticated user (Require authentication)
        /// </summary>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [HttpPatch("DisplayName")]
        public async Task<IActionResult> UpdateDisplayName([FromBody][Required]string newDisplayName)
        {
            var userId = HttpContext.GetUserIdFromHeader();

            var newName = await accountManager.UpdateDisplayNameAsync(userId, newDisplayName);

            return Ok(newName);
        }
    }
}