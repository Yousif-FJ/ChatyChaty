using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerHubSchema.v1;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.ValidationAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatyChaty.Controllers.v1
{
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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [HttpPost("Photo")]
        public async Task<IActionResult> SetPhotoForSelf([FromForm] UploadFileSchema uploadFile)
        {
            var userId = HttpContext.GetUserIdFromHeader();

            var result = await accountManager.SetPhotoAsync(
                userId, 
                uploadFile.PhotoFile.FileName,
                uploadFile.PhotoFile.OpenReadStream());

            if (result.Success == false)
            {
                return BadRequest(new ErrorResponse(result.Errors));
            }

            return Ok(result.URL);

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

            var result = await accountManager.CreateConversationAsync(userId, userName);
            if (result.Error is not null)
            {
                return NotFound(new ErrorResponse(result.Error));
            }

            var response = new UserProfileResponse(result.Conversation.ChatId.Value,
                     new ProfileResponse(result.Conversation.Username,
                      result.Conversation.DisplayName,
                      result.Conversation.PhotoURL));

            return Ok(response);
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