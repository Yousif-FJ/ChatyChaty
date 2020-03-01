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
        private readonly IWebHostEnvironment webHost;
        private readonly IMessageRepository messageRepository;

        public ApiController(ILogger<ApiController> logger , IWebHostEnvironment webHost, IMessageRepository messageRepository)
        {
            _logger = logger;
            this.webHost = webHost;
            this.messageRepository = messageRepository;
        }

        [HttpGet]
        [Route("GetImg")]
        public string GetImg()
        {
            return Path.Combine(webHost.WebRootPath, "img", "CatFilterReaction.jpg");
        }

        [HttpPost]
        [Route("TestingPost")]
        public JsonResult TestingPost([FromBody] Message K)
        {
            return new JsonResult(K);
        }
    }
}
