using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChatyChaty.Model;
using ChatyChaty.Model.MessagesModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatyChaty.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        /// <returns></returns>
        [HttpGet("GetImg")]
        public IActionResult GetImg()
        {
            return Ok($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/CatFilterReaction.jpg");
        }


        /// <summary>
        /// Get an array of all messages.
        /// </summary>
        [HttpGet("GetAllMessages")]
        public IActionResult GetAllMessages()
        {
            return Ok(messageRepository.GetAllMessages());
        }

        /// <summary>
        /// Take a Message object as input paramter and reposnd back the same object.
        /// </summary>
        /// <response code="400">Posted Message object doesn't match schemas</response>   
        [HttpPost("TestingPost")]
        public IActionResult TestingPost([FromBody] Message message)
        {
            return Ok(message);
        }
    }
}
