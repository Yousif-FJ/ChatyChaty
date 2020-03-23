using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerSchema.v2;
using ChatyChaty.Model.OldModel;
using ChatyChaty.ValidationAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatyChaty.Controllers.v2
{
    [Route("api/v2/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class MainController : Controller
    {
        private readonly IMessageRepository1 messageRepository;

        public MainController(IMessageRepository1 messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        /// <summary>
        /// [Use Message controller instead] Post a message (Require authentication).
        /// </summary>
        /// <remarks>
        /// To authorize you get the JWT tokken from the login or the register actions,
        /// then you add the tokken to the header using the authorize button
        /// </remarks>
        /// <response code="400">Posted Message object doesn't match schemas</response>   
        /// <response code="403">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpPost("PostMessage")]
        [Obsolete("Use Message controller instead")]
        public IActionResult PostMessage([FromBody] PostMessageSchemaOld message)
        {
            var UserNameClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);

            var NewMessage = messageRepository.NewMessage(new Message1
            {
                Body = message.Body,
                Sender = UserNameClaim.Value
            });
            return Ok(new ResponseMessageSchemaOld {
            Body = NewMessage.Body,
            ID = NewMessage.ID,
            Sender = NewMessage.Sender
            });
        }
    }
}