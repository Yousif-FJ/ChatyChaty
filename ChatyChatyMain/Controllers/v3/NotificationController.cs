using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerSchema.v3;
using ChatyChaty.Services;
using ChatyChaty.ValidationAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatyChaty.Controllers.v3
{
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize]
    [SuppressAutoModelStateResponse]
    [CustomModelValidationResponse]
    [ApiController]
    [Route("api/v3/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationHandler notificationHandler;

        public NotificationController(INotificationHandler notificationHandler)
        {
            this.notificationHandler = notificationHandler;
        }

        /// <summary>
        /// Check if there is new updates for messages or chats
        /// </summary>
        /// <remarks>
        /// <br>Use GetNewMessages If Messages updates is true, Use GetChat then GetNewMessage If ChatUpdate is true.</br>
        /// <br>Once this method is called the values will be reset for chat and message updates</br>
        /// <br>Example Response:</br>
        /// <br> 
        /// {
        ///  "success": true,
        ///  "errors": null,
        ///  "data": {
        ///     "chatUpdate": true,
        ///     "messageUpdate": false,
        ///     "deliveredUpdate": false
        ///         }
        /// }
        /// </br>
        /// </remarks>
        /// <returns></returns>
        [Authorize]
        [HttpGet("Updates")]
        public async Task<IActionResult> CheckForUpdates()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var result = await notificationHandler.CheckForUpdatesAsync(long.Parse(userId));
            var responseBase = new CheckForUpdatesResponseBase
            {
                ChatUpdate = result.ChatUpdate,
                MessageUpdate = result.MessageUpdate,
                DeliveredUpdate = result.DeliveredUpdate
            };
            return Ok(new ResponseBase<CheckForUpdatesResponseBase>
            {
                Success = true,
                Data = responseBase
            });
        }
    }
}