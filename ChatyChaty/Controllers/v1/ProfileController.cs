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
    [SuppressAutoModelStateResponse]
    [CustomModelValidationResponse]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
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
        /// <remarks>
        /// <br>Example response:</br>
        /// <br>
        /// {
        ///  "*photoUrl*"
        /// }
        /// </br>
        /// </remarks>
        /// <response code="400">The uploaded Photo must be a vaild img with png, jpg or jpeg with less than 4MB size</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>

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
        /// <remarks><br>This is used to start a chat with a user</br>
        /// <br>You may get the DisplayName as null due to account greated before the last change</br>
        /// <br>Example response:</br>
        /// <br>
        /// {
        /// "chatId": 1,
        /// "profile":{
        /// "username": "*UserName*",
        /// "displayName": "*DisplayName*",
        /// "PhotoURL": "*URL*"}
        /// }
        /// </br>
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">When user was found</response>
        /// <response code="400">Model validation error</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>

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
        /// <remarks>
        /// <br>This is used when there is an update in chat info.</br>
        /// <br>Example response:</br>
        /// <br>
        /// [
        ///      {
        /// "chatId": 1,
        /// "profile":{
        /// "username": "*UserName*",
        /// "displayName": "*DisplayName*",
        /// "PhotoURL": "*URL*"}
        ///     }
        /// ]
        /// </br>
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>

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
        /// <remarks>
        /// <br>Example reposne:</br>
        /// <br>
        /// {
        ///  "*newName*"
        /// }
        /// </br>
        /// </remarks>
        /// <param name="newDisplayName"></param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="400">Model validation error</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>

        [HttpPatch("DisplayName")]
        public async Task<IActionResult> UpdateDisplayName([FromBody][Required]string newDisplayName)
        {
            var userId = HttpContext.GetUserIdFromHeader();

            var newName = await accountManager.UpdateDisplayNameAsync(userId, newDisplayName);

            return Ok(newName);
        }
    }
}