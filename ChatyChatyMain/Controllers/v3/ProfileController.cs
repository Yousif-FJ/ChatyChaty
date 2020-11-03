using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerHubSchema.v3;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.ValidationAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatyChaty.Controllers.v3
{
    [ApiExplorerSettings(GroupName = "v2")]
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
        /// <br>Example response:</br>
        /// <br>
        /// {
        ///  "success": true,
        ///  "errors": null,
        ///  "data": "*photoUrl*"
        /// }
        /// </br>
        /// </remarks>
        /// <response code="400">The uploaded Photo must be a vaild img with png, jpg or jpeg with less than 4MB size</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpPost("Photo")]
        public async Task<IActionResult> SetPhotoForSelf([FromForm]UploadFileSchema uploadFile)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var setPhotoResult = await accountManager.SetPhotoAsync(long.Parse(userId),
                uploadFile.PhotoFile.FileName, uploadFile.PhotoFile.OpenReadStream());
            if (setPhotoResult.Success == true)
            {
                return Ok(new ResponseBase<string>
                {
                    Success = true,
                    Data = setPhotoResult.URL
                }); 
            }
            else
            {
                return BadRequest(new ResponseBase<string>
                {
                    Success = false,
                    Errors = setPhotoResult.Errors
                });
            }

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
        ///  "success": true,
        ///  "errors": null,
        ///  "data":{
        ///     "chatId": 1,
        ///     "profile":{
        ///     "username": "*UserName*",
        ///     "displayName": "*DisplayName*",
        ///     "PhotoURL": "*URL*"}
        ///         }
        /// }
        /// </br>
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">When user was found</response>
        /// <response code="400">Model validation error</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpGet("User")]
        public async Task<IActionResult> GetUser([FromHeader]string userName)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var result = await accountManager.NewConversationAsync(long.Parse(userId), userName);
            if (result.Error != null)
            {
                return NotFound(new ResponseBase<GetUserProfileResponseBase>
                {
                    Success = false,
                    Errors = new Collection<string> { result.Error }
                });
            }
            var response = new ResponseBase<GetUserProfileResponseBase>
            {
                Success = true,
                Data = new GetUserProfileResponseBase
                { 
                    ChatId = result.Conversation.ChatId,
                    Profile = new ProfileSchema
                    {
                        DisplayName = result.Conversation.DisplayName,
                        Username = result.Conversation.Username,
                        PhotoURL = result.Conversation.PhotoURL
                    }
                }
            };
            return Ok(response);
        }

        /// <summary>
        /// Get a list of all chat's information like username and ... so on (Require authentication)
        /// </summary>
        /// <remarks>
        /// <br>This is used when there is an update in chat info.</br>
        /// <br>Example response:</br>
        /// <br>
        /// {
        ///  "success": true,
        ///  "errors": null,
        ///  "data":[
        ///         {
        ///     "chatId": 1,
        ///     "profile":{
        ///     "username": "*UserName*",
        ///     "displayName": "*DisplayName*",
        ///     "PhotoURL": "*URL*"}
        ///         }
        ///     ]
        /// }
        /// </br>
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpGet("Chats")]
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
                    ChatId = chat.ChatId,
                    Profile = new ProfileSchema
                    {
                        DisplayName = chat.DisplayName,
                        Username = chat.Username,
                        PhotoURL = chat.PhotoURL
                    }
                }
                    );
            };
            var response = new ResponseBase<IEnumerable<GetUserProfileResponseBase>>()
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
        /// <br>Example reposne:</br>
        /// <br>
        /// {
        ///  "success": true,
        ///  "errors": null,
        ///  "data": "*newName*"
        /// }
        /// </br>
        /// </remarks>
        /// <param name="newDisplayName"></param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="400">Model validation error</response>
        /// <response code="401">Unauthenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpPatch("DisplayName")]
        public async Task<IActionResult> UpdateDisplayName([FromBody]string newDisplayName)
        {
            var UserId = long.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var newName = await accountManager.UpdateDisplayNameAsync(UserId, newDisplayName);
            return Ok(new ResponseBase<string>
            {
                Success = true,
                Data = newName
            });
        }
    }
}