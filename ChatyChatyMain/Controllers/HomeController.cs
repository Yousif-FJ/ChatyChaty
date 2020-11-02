using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ChatyChaty.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

        }
        [Route("/")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/HubTestClient")]
        [HttpGet]
        public IActionResult HubTestClient()
        {
            return View();
        }
    }
}