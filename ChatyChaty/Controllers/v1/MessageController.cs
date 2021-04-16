using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChatyChaty.ControllerHubSchema.v1;
using ChatyChaty.Domain.ApplicationExceptions;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Model.MessagingModel;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.ValidationAttribute;

namespace ChatyChaty.Controllers.v1
{
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize]
    [SuppressAutoModelStateResponse]
    [CustomModelValidationResponse]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;

        public MessageController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet("MessagesForChat")]
        public async Task<IActionResult> GetMessagesForChat([FromQuery][Required] string chatId)
        {
            var userId = HttpContext.GetUserIdFromHeader();

            ConversationId chatIdApp;
            try
            {
                chatIdApp = new ConversationId(chatId);
            }
            catch (InvalidIdFormatException e)
            {
                return BadRequest(new ErrorResponse(e.Message));
            }

            var result = await messageService.GetMessageForChat(userId, chatIdApp);

            if (result.Error is not null)
            {
                return BadRequest(new ErrorResponse(result.Error));
            }

            var messages = result.Messages.ToMessageInfoResponse(userId);

            return Ok(new List<MessageInfoReponse>(messages));
        }


        /// <summary>
        /// Get new messages by supplying the last messageId of the last chat or null if no messages (Require authentication)
        /// </summary>
        /// <response code="200"></response>
        /// <response code="400">Array of error messages </response>
        /// <response code="401">Not Authenticated</response>
        [HttpGet("NewMessages")]
        public async Task<IActionResult> GetNewMessages([FromQuery] string lastMessageId)
        {
            var userId = HttpContext.GetUserIdFromHeader();

            GetMessagesModel result;
            if (string.IsNullOrEmpty(lastMessageId))
            {
                result = await messageService.GetNewMessages(userId, null);
            }
            else
            {
                MessageId messageIdApp;
                try
                {
                    messageIdApp = new MessageId(lastMessageId);
                }
                catch (InvalidIdFormatException e)
                {
                    return BadRequest(new ErrorResponse(e.Message));
                }

                result = await messageService.GetNewMessages(userId, messageIdApp);
            }

            //the error never a value
            if (result.Error != null)
            {
                return BadRequest(new ErrorResponse(result.Error));
            }

            var messages = result.Messages.ToMessageInfoResponse(userId);

            return Ok(new List<MessageInfoReponse>(messages));
        }


        /// <summary>
        /// Check if the message is Delivered or not (Require authentication)
        /// </summary>
        /// <remarks>
        /// <br>Example response:</br>
        /// <br>
        /// {
        ///  true
        ///  }
        /// </br>
        /// </remarks>
        /// <response code="200">A bool whether the message is Delivered or not</response>
        /// <response code="400">The user doesn't own the message or invalid MessageId</response>
        /// <response code="401">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("Delivered")]
        public async Task<IActionResult> CheckDelivered([FromQuery][Required] string messageId)
        {
            var userId = HttpContext.GetUserIdFromHeader();

            MessageId messageIdApp;
            try
            {
                messageIdApp = new MessageId(messageId);
            }
            catch (InvalidIdFormatException e)
            {
                return BadRequest(new ErrorResponse(e.Message));
            }

            var result = await messageService.IsDelivered(userId, messageIdApp);

            if (result.Error is not null)
            {
                return BadRequest(new ErrorResponse(result.Error));
            }
            return Ok(result.IsDelivered);
        }


        /// <summary>
        /// Send New message with the chatId(Require authentication)
        /// </summary>
        /// <remarks>
        /// <br>Example response:</br>
        /// <br>
        /// {
        ///    "chatId": 1,
        ///    "messageId": 1,
        ///    "sender": "*Username*",
        ///    "body": "*The message*",
        ///    "delivered": null
        /// }
        /// </br>
        /// </remarks>
        /// <response code="200">sent! You get the message back in the response</response>
        /// <response code="400">The user doesn't own the chat</response>
        /// <response code="401">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("Message")]
        public async Task<IActionResult> SendMessage([FromBody]SendMessageSchema messageSchema)
        {
            var userId = HttpContext.GetUserIdFromHeader();


            ConversationId chatIdApp;
            try
            {
                chatIdApp = new ConversationId(messageSchema.ChatId);
            }
            catch (InvalidIdFormatException e)
            {
                return BadRequest(new ErrorResponse(e.Message));
            }

            var result = await messageService.SendMessage(chatIdApp, userId, messageSchema.Body);

            if (result.Error != null)
            {
                return BadRequest(new ErrorResponse(result.Error));
            }

            var userNameClaim = HttpContext.User.Claims.FirstOrDefault(
                claim => claim.Type == ClaimTypes.Name);

            var response = new MessageInfoReponse(
                result.Message.ConversationId.Value,
                result.Message.Id.Value,
                userNameClaim.Value,
                result.Message.Body,
                false);

            return Ok(response);
        }
    }
}