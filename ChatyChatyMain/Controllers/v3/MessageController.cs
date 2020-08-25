using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerHubSchema.v3;
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
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;

        public MessageController(IMessageService messageService)
        {
            this.messageService = messageService;
        }


        /// <summary>
        /// Get new messages by supplying the last messageId of the last chat (Require authentication)
        /// </summary>
        /// <remarks>
        /// <br>Use CheckForNewMessages first for performance.</br>
        /// <br>You may supply 0 as last messageId if there are no messages.</br>
        /// <br>Expect delivered to be null for the message you received.</br>
        /// <br>You may get the full chat info (like sender username and picture) using the ChatId,
        /// You can use the action GetChatInfo to get the chat info.</br>
        /// Example response:
        /// <br>
        /// {
        ///  "success": true,
        ///  "errors": null,
        ///  "data":[
        ///  {
        ///    "chatId": 1,
        ///    "messageId": 1,
        ///    "sender": "*Username*",
        ///    "body": "*The message*",
        ///    "delivered": true
        ///  }]
        /// }
        /// </br>
        /// </remarks>
        /// <param name="lastMessageId"></param>
        /// <returns></returns>
        /// <response code="200">An array of messages</response>
        /// <response code="400">Invalid MessageId</response>
        /// <response code="401">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("NewMessages")]
        public async Task<IActionResult> GetNewMessages([FromHeader]long lastMessageId)
        {
            var userId = long.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var result = await messageService.GetNewMessages(userId, lastMessageId);
            //the error never a value
            if (result.Error != null)
            {
                return BadRequest(new ResponseBase<IEnumerable<MessageInfoBase>>
                {
                    Success = false,
                    Errors = new Collection<string>() { result.Error }
                });
            }
            var Messages = new List<MessageInfoBase>();
            foreach (var message in result.Messages)
            {
                Messages.Add(new MessageInfoBase
                {
                    Body = message.Body,
                    ChatId = message.ConversationId,
                    MessageId = message.Id,
                    Sender = message.Sender.UserName,
                    Delivered = message.SenderId == userId ? message.Delivered : (bool?)null
                }); ;
            }
            return Ok(new ResponseBase<IEnumerable<MessageInfoBase>>
            {
                Success = true,
                Data = Messages
            });
        }


        /// <summary>
        /// Check if the message is Delivered or not (Require authentication)
        /// </summary>
        /// <remarks>
        /// <br>Only works if the message was sent by the user himself.</br>
        /// <br>should be only used when the chat is open, to reduce performance hit</br>
        /// <br>Example response:</br>
        /// <br>
        /// {
        ///  "success": true,
        ///  "errors": null,
        ///  "data": true
        ///  }
        /// </br>
        /// </remarks>
        /// <param name="messageId">long represent the message Id</param>
        /// <returns></returns>
        /// <response code="200">A bool whether the message is Delivered or not</response>
        /// <response code="400">The user doesn't own the message or invalid MessageId</response>
        /// <response code="401">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("Delivered")]
        public async Task<IActionResult> CheckDelivered([FromHeader]long messageId)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var result = await messageService.IsDelivered(long.Parse(userIdClaim.Value), messageId);
            if (result.Error != null)
            {
                return BadRequest(new ResponseBase<bool?>
                {
                    Success = false,
                    Errors = new Collection<string> { result.Error }
                });
            }
            return Ok(new ResponseBase<bool?>
            {
                Success = true,
                Data = result.IsDelivered
            });
        }


        /// <summary>
        /// Send New message with the chatId(Require authentication)
        /// </summary>
        /// <remarks>
        /// <br>you can get the chatId using the action GetUser,
        /// and get the chat info from the action GetChatInfo.</br>
        /// <br>Example response:</br>
        /// <br>
        /// {       
        ///  "success": true,
        ///  "errors": null,
        ///  "data": {
        ///    "chatId": 1,
        ///    "messageId": 1,
        ///    "sender": "*Username*",
        ///    "body": "*The message*",
        ///    "delivered": null
        ///         }
        /// }
        /// </br>
        /// </remarks>
        /// <param name="messageSchema">Object representing the message info</param>
        /// <returns></returns>
        /// <response code="200">sent! You get the message back in the response</response>
        /// <response code="400">The user doesn't own the chat</response>
        /// <response code="401">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("Message")]
        public async Task<IActionResult> SendMessage([FromBody]SendMessageSchema messageSchema)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(
                claim => claim.Type == ClaimTypes.NameIdentifier);
            var result = await messageService.SendMessage(messageSchema.ChatId,
                long.Parse(userIdClaim.Value), messageSchema.Body);

            if (result.Error != null)
            {
                return BadRequest(new ResponseBase<MessageInfoBase>
                {
                    Success = false,
                    Errors = new Collection<string> { result.Error}
                });
            }
            var userNameClaim = HttpContext.User.Claims.FirstOrDefault(
                claim => claim.Type == ClaimTypes.Name);
            var responseBase = new MessageInfoBase
            {
                Body = result.Message.Body,
                MessageId = result.Message.Id,
                ChatId = result.Message.ConversationId,
                Sender = userNameClaim.Value,
                Delivered = false
            };
            return Ok(new ResponseBase<MessageInfoBase>
            {
                Success = true,
                Data = responseBase
            });
        }
    }
}