using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChatyChaty.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatyChaty.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ApiController : ControllerBase
    {

        private readonly ILogger<ApiController> _logger;
        private readonly IMessageRepository messageRepository;

        public ApiController(ILogger<ApiController> logger , IMessageRepository messageRepository)
        {
            _logger = logger;
            this.messageRepository = messageRepository;
        }

        [HttpGet]
        [Route("GetImg")]
        public string GetImg()
        {
            return "CatFilterReaction.jpg";
        }

        [HttpPost]
        [Route("TestingPost")]
        public JsonResult TestingPost([FromBody] Message K)
        {
            return new JsonResult(K);
        }
    }
}
