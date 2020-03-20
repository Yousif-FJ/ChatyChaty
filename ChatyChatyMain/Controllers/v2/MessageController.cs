using System;
using System.Collections.Generic;
using System.Linq;
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
    [RequireHttpsOrClose]
    [ApiController]
    [Route("api/v1/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class MessageController : ControllerBase
    {
        private readonly MessageService messageService;
        private readonly AccountManager accountManager;

        public MessageController(MessageService messageService, AccountManager accountManager)
        {
            this.messageService = messageService;
            this.accountManager = accountManager;
        }

        IActionResult CheckForNewMessages(long SumOfMIdAndCId)
        {
            //not implmented
            return Ok(true);
        }

        IActionResult GetNewMessages(IEnumerable<ConversationMessageID> conversationMessageIDs)
        {
            //not implmented
            return Ok();
        }

        [Authorize]
        [HttpGet("CheckDelivered")]
        public IActionResult CheckDelivered(long MessageId)
        {
            //not implmented
            return Ok(true);
        }

        [Authorize]
        [HttpPost("SendMessage")]
        public IActionResult SendMessage(SendMessageSchema messageSchema)
        {
            //not implmented
            if (messageSchema.ConversationId != null)
            {

            }
            return Ok();
        }

    }
}