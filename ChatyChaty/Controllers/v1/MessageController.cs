using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChatyChaty.Domain.ApplicationExceptions;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Services.MessageServices;
using ChatyChaty.ModelExtensions;
using ChatyChaty.HttpShemas.v1.Message;
using ChatyChaty.HttpShemas.v1.Error;

namespace ChatyChaty.Controllers.v1
{
    [Authorize]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;

        public MessageController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        /// <summary>
        /// Return the messages of a specific chat, either load chats with this method,
        /// Or use getNewMessages to load messages from all chats. Don't mix them as it will cause problems.
        /// </summary>
        [ProducesResponseType(typeof(List<MessageResponse>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [HttpGet("MessagesForChat")]
        public async Task<IActionResult> GetMessagesForChat([FromQuery][Required] string chatId)
        {
            var userId = HttpContext.GetUserIdFromHeader();

            try
            {
                var chatIdApp = new ConversationId(chatId);
                var result = await messageService.GetMessageForChat(userId, chatIdApp);
                var messages = result.ToMessageInfoResponse(userId);

                return Ok(new List<MessageResponse>(messages));
            }
            catch (Exception e) when (e is InvalidEntityIdException || e is InvalidIdFormatException)
            {
                return BadRequest(new ErrorResponse(e.Message));
            }
        }


        /// <summary>
        /// Get new messages by supplying the last messageId of the last chat or null if no messages (Require authentication)
        /// </summary>
        [ProducesResponseType(typeof(List<MessageResponse>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [HttpGet("NewMessages")]
        public async Task<IActionResult> GetNewMessages([FromQuery] DateTime? lastMessageDateTime)
        {
            var userId = HttpContext.GetUserIdFromHeader();

            IList<Message> messages;
            if (lastMessageDateTime is null)
            {
                messages = await messageService.GetNewMessages(userId, null);
            }
            else
            {
                messages = await messageService.GetNewMessages(userId, lastMessageDateTime);
            }

            var messagesRespond = messages.ToMessageInfoResponse(userId);

            return Ok(messagesRespond);
        }


        /// <summary>
        /// Check if the message is Delivered or not (Require authentication)
        /// </summary>
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [HttpGet("Delivered")]
        public async Task<IActionResult> CheckDelivered([FromQuery][Required] string messageId)
        {
            var userId = HttpContext.GetUserIdFromHeader();

            try
            {
                var messageIdApp = new MessageId(messageId);
                var IsDelivered = await messageService.IsDelivered(userId, messageIdApp);

                return Ok(IsDelivered);
            }
            catch (Exception e) when (e is InvalidEntityIdException || e is InvalidIdFormatException)
            {
                return BadRequest(new ErrorResponse(e.Message));
            }
        }


        /// <summary>
        /// Send New message with the chatId(Require authentication)
        /// </summary>
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType(typeof(ErrorResponse))]
        [HttpPost("Message")]
        public async Task<IActionResult> SendMessage([FromBody]SendMessageSchema messageSchema)
        {
            var userId = HttpContext.GetUserIdFromHeader();

            try
            {
                var chatIdApp = new ConversationId(messageSchema.ChatId);
                var result = await messageService.SendMessage(chatIdApp, userId, messageSchema.Body);

                var userNameClaim = HttpContext.User.Claims.FirstOrDefault(
                    claim => claim.Type == ClaimTypes.Name);

                var response = new MessageResponse(
                    result.ConversationId.Value,
                    result.Id.Value,
                    userNameClaim.Value,
                    result.Body,
                    result.SentTime,
                    result.StatusUpdateTime,
                    false);

                return Ok(response);
            }

            catch (Exception e) when (e is InvalidEntityIdException || e is InvalidIdFormatException)
            {
                return BadRequest(new ErrorResponse(e.Message));
            }
        }
    }
}