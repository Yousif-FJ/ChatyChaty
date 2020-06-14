using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerSchema.v2;
using ChatyChaty.Model.NotficationHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatyChaty.Controllers.v2
{
    [Authorize]
    [ApiController]
    [Route("api/v2/[controller]")]
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
        /// "chatUpdate": true,
        /// "messageUpdate": false,
        /// "deliveredUpdate": false
        /// }
        /// </br>
        /// </remarks>
        /// <returns></returns>
        [Authorize]
        [HttpGet("CheckForUpdates")]
        [Obsolete]
        public async Task<IActionResult> CheckForUpdates()
        {
            var UserId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var result = await notificationHandler.CheckForUpdates(long.Parse(UserId));
            var response = new CheckForUpdatesResponse
            {
                ChatUpdate = result.ChatUpdate,
                MessageUpdate = result.MessageUpdate,
                DeliveredUpdate = result.DeliveredUpdate
            };
            return Ok(response);
        }
    }
}