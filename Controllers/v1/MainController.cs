using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.Model.ControllerSchema.v1;
using ChatyChaty.Model.MessageModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatyChaty.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class MainController : ControllerBase
    {

        private readonly ILogger<MainController> _logger;
        private readonly IMessageRepository messageRepository;

        public MainController(ILogger<MainController> logger , IMessageRepository messageRepository)
        {
            _logger = logger;
            this.messageRepository = messageRepository;
        }


        /// <summary>
        /// Get the location of an img file
        /// </summary>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("GetImg")]
        public IActionResult GetImg()
        {
            return Ok($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/CatFilterReaction.jpg");
        }


        /// <summary>
        /// Get an array of all messages.
        /// </summary>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("GetAllMessages")]
        public IActionResult GetAllMessages()
        {
            return Ok(messageRepository.GetAllMessages());
        }

        /// <summary>
        /// Post a message (Require authentication).
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
        public IActionResult PostMessage([FromBody] MessageSchema message)
        {
            var UserNameClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);

            var NewMessage = messageRepository.NewMessage(new Message
            {
                Body = message.Body,
                Sender = UserNameClaim.Value
            });
            return Ok(NewMessage);
        }
    }
}
