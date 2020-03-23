using System;
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
    [RequireHttpsOrClose]
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


        [HttpGet("CheckForNewMessages")]
        public async Task<IActionResult> CheckForNewMessages([FromBody]long LastMessageId)
        {
            var UserIdClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var result = await messageService.CheckForNewMessages(long.Parse(UserIdClaim.Value), LastMessageId);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result.Value);
        }


        [HttpGet("GetNewMessages")]
        public async Task<IActionResult> GetNewMessages([FromBody]long LastMessageId)
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
                    Sender = (await accountManager.GetUser(message.SenderId)).UserName,
                    Delivered = message.SenderId == UserId ? message.Delivered : (bool?)null
                });
            }
            return Ok(Messages);
        }


        /// <summary>
        /// Check if the message is Delivered or not
        /// </summary>
        /// <remarks>Only works if the message was sent by the user himself,
        /// should be only used when the conversation is open for performance</remarks>
        /// <param name="MessageId">long represent the message Id</param>
        /// <returns></returns>
        /// <response code="200">A bool whether the message is Delivered or not</response>
        /// <response code="400">The user doesn't own the message or invalid MessageId</response>
        /// <response code="403">Not Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("CheckDelivered")]
        public async Task<IActionResult> CheckDelivered([FromBody]long MessageId)
        {
            var UserIdClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var Result = await messageService.IsDelivered(long.Parse(UserIdClaim.Value), MessageId);
            if (Result == null)
            {
                return BadRequest();
            }
            return Ok(Result.Value);
        }


        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody]SendMessageSchema messageSchema)
        {
            var UserIdClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            var message = await messageService.SendMessage(messageSchema.ConversationId, long.Parse(UserIdClaim.Value), messageSchema.Body);

            if (message == null)
            {
                return BadRequest();
            }
            var UserNameClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
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