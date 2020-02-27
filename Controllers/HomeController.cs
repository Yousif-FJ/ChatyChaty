using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatyChaty.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment webHost;

        public HomeController(ILogger<HomeController> logger , IWebHostEnvironment webHost )
        {
            _logger = logger;
            this.webHost = webHost;
        }

        [HttpGet]
        [Route("Index")]
        public string Index()
        {
           _logger.LogInformation(Environment.GetEnvironmentVariable("DATABASE_URL"));
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
