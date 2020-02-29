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
        [Route("Index")]
        public string Index()
        {
           _logger.LogInformation($"The first message body is {messageRepository.GetAllMessages().FirstOrDefault().Body}");
            return "App running";
        }


        [HttpGet]
        [Route("GetImg")]
        public string GetImg()
        {
            return Path.Combine(webHost.WebRootPath, "img", "CatFilterReaction.jpg");
        }
    }
}
