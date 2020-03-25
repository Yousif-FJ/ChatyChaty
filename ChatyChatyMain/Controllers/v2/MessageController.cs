﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatyChaty.ControllerSchema.v1;
using ChatyChaty.ControllerSchema.v2;
using ChatyChaty.Model.DBModel;
using ChatyChaty.Services;
using ChatyChaty.ValidationAttribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatyChaty.Controllers.v2
{
    [Authorize]
    [ApiController]
    [Route("api/v2/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;
        private readonly IAccountManager accountManager;

        public MessageController(IMessageService messageService, IAccountManager accountManager)
        {
            this.messageService = messageService;
            this.accountManager = accountManager;
        }

        /// <summary>
        /// Check for new messages by supplying the last messageId of the last conversation (Require authentication)
        /// </summary>
        /// <remarks>
        /// <br>You may supply 0 as last messageId if there are no messages.</br>
        /// <br>Example response: false</br>
        /// </remarks>
        /// <param name="LastMessageId"></param>
        /// <returns></returns>
        /// <response code="200">A bool whether there is a new message or not</response>
        /// <response code="400">Invalid MessageId</response>
        /// <response code="403">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("CheckForNewMessages")]
        public async Task<IActionResult> CheckForNewMessages([FromHeader]long LastMessageId)
        {
            var UserIdClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var result = await messageService.CheckForNewMessages(long.Parse(UserIdClaim.Value), LastMessageId);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Get new messages by supplying the last messageId of the last conversation (Require authentication)
        /// </summary>
        /// <remarks>
        /// You should use CheckForNewMessages first for performance (I'm not sure if it makes much difference will test latter).
        /// You may supply 0 as last messageId if there are no messages.
        /// Expect delivered to be null for the message you received.
        /// You may get the full conversation info (like sender username and picture) using the conversationId,
        /// You can use the action GetConversationInfo to get the conversation info. 
        /// Example response:
        /// <br>
        /// [
        ///  {
        ///    "conversationId": 1,
        ///    "messageId": 1,
        ///    "sender": "*Username*",
        ///    "body": "*The message*",
        ///    "delivered": true
        ///  }
        /// ]
        /// </br>
        /// </remarks>
        /// <param name="LastMessageId"></param>
        /// <returns></returns>
        /// <response code="200">An array of messages</response>
        /// <response code="400">Invalid MessageId</response>
        /// <response code="403">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("GetNewMessages")]
        public async Task<IActionResult> GetNewMessages([FromHeader]long LastMessageId)
        {
            var UserId = long.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var result = await messageService.GetNewMessages(UserId, LastMessageId);
            if (result == null)
            {
                return BadRequest();
            }
            var Messages = new List<NewMessagesResponse>();
            foreach (var message in result)
            {
                Messages.Add(new NewMessagesResponse
                {
                    Body = message.Body,
                    ConversationId = message.ConversationId,
                    MessageId = message.Id,
                    Sender = message.Sender.UserName,
                    Delivered = message.SenderId == UserId ? message.Delivered : (bool?)null
                });
            }
            return Ok(Messages);
        }


        /// <summary>
        /// Check if the message is Delivered or not (Require authentication)
        /// </summary>
        /// <remarks>Only works if the message was sent by the user himself.
        /// should be only used when the conversation is open, to reduce performance hit
        /// <br>Example response: false</br>
        /// </remarks>
        /// <param name="MessageId">long represent the message Id</param>
        /// <returns></returns>
        /// <response code="200">A bool whether the message is Delivered or not</response>
        /// <response code="400">The user doesn't own the message or invalid MessageId</response>
        /// <response code="403">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("CheckDelivered")]
        public async Task<IActionResult> CheckDelivered([FromHeader]long MessageId)
        {
            var UserIdClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var Result = await messageService.IsDelivered(long.Parse(UserIdClaim.Value), MessageId);
            if (Result == null)
            {
                return BadRequest();
            }
            return Ok(Result.Value);
        }


        /// <summary>
        /// Send New message with the conversationId(Require authentication)
        /// </summary>
        /// <remarks>you can get the conversationId using the action GetUser,
        /// and get the conversation info from the action GetConversationInfo.
        /// <br>Example response:</br>
        /// <br>
        ///    {
        ///    "conversationId": 1,
        ///    "messageId": 1,
        ///    "sender": "*Username*",
        ///    "body": "*The message*",
        ///    "delivered": null
        ///    }
        /// </br>
        /// </remarks>
        /// <param name="messageSchema">Object representing the message info</param>
        /// <returns></returns>
        /// <response code="200">sent! You get the message back in the response</response>
        /// <response code="400">The user doesn't own the conversation</response>
        /// <response code="403">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody]SendMessageSchema messageSchema)
        {
            var UserIdClaim = HttpContext.User.Claims.FirstOrDefault(
                claim => claim.Type == ClaimTypes.NameIdentifier);
            var message = await messageService.SendMessage(messageSchema.ConversationId,
                long.Parse(UserIdClaim.Value), messageSchema.Body);

            if (message == null)
            {
                return BadRequest();
            }
            var UserNameClaim = HttpContext.User.Claims.FirstOrDefault(
                claim => claim.Type == ClaimTypes.Name);
            var response = new NewMessagesResponse
            {
                Body = message.Body,
                MessageId = message.Id,
                ConversationId = message.ConversationId,
                Sender = UserNameClaim.Value
            };
            return Ok(response);
        }
    }
}