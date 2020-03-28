using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerSchema.v1;
using ChatyChaty.Services;
using ChatyChaty.ValidationAttribute;
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
        public async Task<IActionResult> SetPhotoForSelf([FromForm]UploadFileSchema uploadFile)
        {         
            var UserNameClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            var user = await accountManager.GetUser(UserNameClaim.Value);
            await pictureProvider.ChangePhoto(user.Id,user.UserName, uploadFile.PhotoFile);

            var URL = await pictureProvider.GetPhotoURL(user.Id,user.UserName);
                return Ok(URL);
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
        /// <param name="UserName"></param>
        /// <returns></returns>
        /// <response code="200">When user was found or not</response>
        /// <response code="401">Unaithenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromHeader]string UserName)
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
                ChatId = conversationId,
                Profile = new ProfileResponse
                {
                    DisplayName = user.DisplayName,
                    Username = user.UserName,
                    PhotoURL = await pictureProvider.GetPhotoURL(user.Id, user.UserName)
                }
            };
            return Ok(response);
        }

        /*
        /// <summary>
        /// Get chat information like username and ... (Require authentication)
        /// </summary>
        /// <remarks>This is used when GetNewMessages return a message with a chat Id, 
        /// it also should be used everytime a chat is opened to keep the profile upto date (DisplayName can be changed)
        /// <br>
        /// Example response:
        /// {
        ///  "chatId": 1,
        ///  "profile":{
        ///  "username": "*UserName*",
        ///  "displayName": "*DisplayName*",
        ///  "PhotoURL": "*URL*"}
        /// }
        /// </br>
        /// </remarks>
        /// <param name="ChatId"></param>
        /// <returns></returns>
        /// <response code="400">Requested ChatId doesn't exist</response>
        /// <response code="401">Unaithenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpGet("GetChatInfo")]
        public async Task<IActionResult> GetChatInfo([FromHeader]long ChatId)
        {
            var UserIdClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var conversation = await messageService.GetConversationInfo(long.Parse(UserIdClaim.Value), ChatId);
            if (conversation == null)
            {
                return BadRequest();
            }
            var response = new GetChatInfoResponse
            {
                ChatId = conversation.ConversationId,
                Profile = new ProfileResponse
                {
                    DisplayName = conversation.SecondUserDisplayName,
                    Username = conversation.SecondUserUsername,
                    PhotoURL = await pictureProvider.GetPhotoURL(conversation.SecondUserId, conversation.SecondUserUsername)
                }
            };
            return Ok(response);
        }
        */

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
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpPatch("UpdateDisplayName")]
        public async Task<IActionResult> UpdateDisplayName([FromBody]string NewDisplayName)
        {
            var UserId = long.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var result = await accountManager.UpdateDisplayName(UserId, NewDisplayName);
            return Ok(result);
        }
    }
}