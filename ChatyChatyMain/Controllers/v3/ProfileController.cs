using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using ChatyChaty.ControllerSchema.v3;
using ChatyChaty.Services;
using ChatyChaty.ValidationAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.ValidationAttributes;

namespace ChatyChaty.Controllers.v3
{
    [SuppressAutoModelStateResponse]
    [CustomModelValidationResponse]
    [Route("api/v3/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IAccountManager accountManager;
        private readonly IMessageService messageService;

        public ProfileController(IAccountManager accountManager, IMessageService messageService)
        {
            this.accountManager = accountManager;
            this.messageService = messageService;
        }


        /// <summary>
        /// Set photo or replace existing one (Require authentication)
        /// </summary>
        /// <remarks>
        /// Return the photo URL as a string (surrounded by "")</remarks>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        /// <response code="400">The uploaded Photo must be a vaild img with png, jpg or jpeg with less than 4MB size</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [HttpPost("SetPhotoForSelf")]
        public async Task<IActionResult> SetPhotoForSelf([FromForm]UploadFileSchema uploadFile)
        {
            var UserId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var URL = await accountManager.SetPhoto(long.Parse(UserId), uploadFile.PhotoFile);
            var response = new ResponseBase
            {
                Data = URL,
                Success = true
            };
                return Ok(response);
        }


        /// <summary>
        /// Find users and return user profile with a chat Id,
        /// which is used to send messages and get other users info(Require authentication)
        /// </summary>
        /// <remarks><br>This is used to start a chat with a user</br>
        /// <br>You may get the DisplayName as null due to account greated before the last change</br>
        /// Example response:
        /// {
        ///  "success": true,
        ///  "error": null,
        ///  "chatId": 1,
        ///  "profile":{
        ///  "username": "*UserName*",
        ///  "displayName": "*DisplayName*",
        ///  "PhotoURL": "*URL*"}
        /// }
        /// </remarks>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// <response code="200">When user was found or not</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromHeader]string userName)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var result = await accountManager.NewConversation(long.Parse(userId), userName);
            if (result.Error != null)
            {
                return Ok(new GetUserProfileResponse
                {
                    Success = false,
                    Errors = new Collection<string> { result.Error }
                });
            }
            var response = new GetUserProfileResponse
            {
                Success = true,
                Data = new GetUserProfileResponseBase
                { 
                    ChatId = result.Conversation.ConversationId,
                    Profile = new ProfileSchema
                    {
                        DisplayName = result.Conversation.SecondUserDisplayName,
                        Username = result.Conversation.SecondUserUsername,
                        PhotoURL = result.Conversation.SecondUserPhoto
                    }
                }
            };
            return Ok(response);
        }

        /// <summary>
        /// Get a list of all chat's information like username and ... (Require authentication)
        /// </summary>
        /// <remarks>This is used when there is an update in chat info
        /// <br>
        ///
        /// </br>
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpGet("GetChats")]
        public async Task<IActionResult> GetChats()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var chats = await messageService.GetConversations(long.Parse(userId));

            var chatList = new List<GetUserProfileResponseBase>();

            foreach (var chat in chats)
            {
                chatList.Add(
                new GetUserProfileResponseBase
                {
                    ChatId = chat.ConversationId,
                    Profile = new ProfileSchema
                    {
                        DisplayName = chat.SecondUserDisplayName,
                        Username = chat.SecondUserUsername,
                        PhotoURL = chat.SecondUserPhoto
                    }
                }
                    );
            };
            var response = new GetChatsResponse()
            {
                Success = true,
                Data = chatList
            };
            return Ok(response);
        }


        /// <summary>
        /// Set or update the DisplayName of the authenticated user (Require authentication)
        /// </summary>
        /// <remarks>
        /// Take the name as a json string (surrounded by "") and
        /// Return the name as a json string (surrounded by "")
        /// </remarks>
        /// <param name="NewDisplayName"></param>
        /// <returns></returns>
        /// <reposne  code="200">Success</reposne>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpPatch("UpdateDisplayName")]
        public async Task<IActionResult> UpdateDisplayName([FromBody]string NewDisplayName)
        {
            var UserId = long.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var newName = await accountManager.UpdateDisplayName(UserId, NewDisplayName);
            return Ok(newName);
        }
    }
}