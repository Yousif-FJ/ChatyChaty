using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerHubSchema.v1;
using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Services.AccountServices;
using ChatyChaty.Domain.Services.MessageServices;
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
        private readonly IPictureProvider pictureProvider;
        private readonly IMessageService messageService;

        public ProfileController(IAccountManager accountManager, IPictureProvider pictureProvider, IMessageService messageService)
        {
            this.accountManager = accountManager;
            this.pictureProvider = pictureProvider;
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
        /// <response code="401">Unaithenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [HttpPost("SetPhotoForSelf")]
        [Obsolete]
        public async Task<IActionResult> SetPhotoForSelf([FromForm]UploadFileSchema uploadFile)
        {
            var UserId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var URL = await accountManager.SetPhotoAsync(long.Parse(UserId),uploadFile.PhotoFile.FileName ,uploadFile.PhotoFile.OpenReadStream());

                return Ok(URL);
        }


        /// <summary>
        /// Find users and return user profile with a chat Id,
        /// which is used to send messages and get other users info(Require authentication)
        /// </summary>
        /// <remarks><br>This is used to start a chat with a user</br>
        /// <br>You may get the DisplayName as null due to account greated before the last change</br>
        /// Example response:
        /// <br>
        /// {
        ///  "success": true,
        ///  "error": null,
        ///  "chatId": 1,
        ///  "profile":{
        ///  "username": "*UserName*",
        ///  "displayName": "*DisplayName*",
        ///  "PhotoURL": "*URL*"}
        /// }
        /// </br>
        /// </remarks>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// <response code="200">When user was found or not</response>
        /// <response code="401">Unaithenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpGet("GetUser")]
        [Obsolete]
        public async Task<IActionResult> GetUser([FromHeader]string userName)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var result = await accountManager.NewConversationAsync(long.Parse(userId), userName);
            if (result.Error != null)
            {
                return Ok(
                    new GetUserProfileResponse
                    {
                        Success = false,
                        Error = result.Error
                    });
            }
            var response = new GetUserProfileResponse
            {
                Success = true,
                ChatId = result.Conversation.ConversationId,
                Profile = new ProfileResponse
                {
                    DisplayName = result.Conversation.SecondUserDisplayName,
                    Username = result.Conversation.SecondUserUsername,
                    PhotoURL = result.Conversation.SecondUserPhoto
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
        /// <response code="401">Unaithenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpGet("GetChats")]
        [Obsolete]
        public async Task<IActionResult> GetChats()
        {
            var UserId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var conversations = await messageService.GetConversations(long.Parse(UserId));

            var response = new List<GetChatInfoResponse>();

            foreach (var conversation in conversations)
            {
                response.Add(
                new GetChatInfoResponse
                {
                    ChatId = conversation.ConversationId,
                    Profile = new ProfileResponse
                    {
                        DisplayName = conversation.SecondUserDisplayName,
                        Username = conversation.SecondUserUsername,
                        PhotoURL = await pictureProvider.GetPhotoURL(conversation.SecondUserId, conversation.SecondUserUsername)
                    }
                }
                    );
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
        [Authorize]
        [HttpPatch("UpdateDisplayName")]
        [Obsolete]
        public async Task<IActionResult> UpdateDisplayName([FromBody]string NewDisplayName)
        {
            var UserId = long.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var result = await accountManager.UpdateDisplayNameAsync(UserId, NewDisplayName);
            return Ok(result);
        }
    }
}