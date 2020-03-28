﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerSchema.v1;
using ChatyChaty.ControllerSchema.v2;
using ChatyChaty.Model.OldModel;
using ChatyChaty.ValidationAttribute;
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
        private readonly IMessageRepository1 messageRepository;

        public MainController(IMessageRepository1 messageRepository)
        {
            this.messageRepository = messageRepository;
        }


        /// <summary>
        /// [Use profile controller] Get the location of an img file
        /// </summary>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("GetImg")]
        [Obsolete("Use profile controller")]
        public IActionResult GetImg()
        {
            return Ok($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/CatFilterReaction.jpg");
        }


        /// <summary>
        /// [use message controller instead] Get an array of all messages.
        /// </summary>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("GetAllMessages")]
        [Obsolete("use message controller instead")]
        public IActionResult GetAllMessages()
        {
            
            return Ok(MessageModelToSchema(messageRepository.GetAllMessages()));
        }


        /// <summary>
        /// [use message controller instead] Get new messages after the specified message ID.
        /// </summary>
        /// <response code="400">ID is out of range</response>   
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("GetNewMessages")]
        [Obsolete("use message controller instead")]
        public IActionResult GetNewMessages([FromQuery]long id)
        {
            var Response = MessageModelToSchema(messageRepository.GetNewMessages(id));
            if (Response != null)
            {
                return Ok(Response);
            }
            else
            {
                return BadRequest("ID is out of range");
            }
        }

        /// <summary>
        /// [use message controller instead] Post a message (Require authentication).
        /// </summary>
        /// <remarks>
        /// To authorize you get the JWT tokken from the login or the register actions,
        /// then you add the tokken to the header using the authorize button
        /// </remarks>
        /// <response code="400">Posted Message object doesn't match schemas</response>   
        /// <response code="403">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Obsolete("use message controller instead")]
        [Authorize]
        [HttpPost("PostMessage")]
        public IActionResult PostMessage([FromBody] MessageSchemaOld message)
        {
            var UserNameClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);

            var NewMessage = messageRepository.NewMessage(new Message1
            {
                Body = message.Body,
                Sender = UserNameClaim.Value
            });
            return Ok(NewMessage);
        }

        /// <summary>
        /// [Not active on the new repository] Delete all messages simple clean and fast :p (Require authentication).
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("DeleteAllMessages")]
        [Obsolete("Not active")]
        public IActionResult DeleteAllMessages()
        {
            messageRepository.DeleteAllMessages();
            return Ok();
        }

        private List<ResponseMessageSchemaOld> MessageModelToSchema(IEnumerable<Message1> MessageSet)
        {
            var responseMessages = new List<ResponseMessageSchemaOld>();
            foreach (var item in MessageSet)
            {
                responseMessages.Add(new ResponseMessageSchemaOld
                {
                    ID = item.ID,
                    Sender = item.Sender,
                    Body = item.Body
                });
            }
            return responseMessages;
        }
    }
}
