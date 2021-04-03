using ChatyChaty.ControllerHubSchema.v1;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// This action is ran when an exception happens in the server
        /// </summary>
        /// <response code="200">(This method actually never retrun 200)</response>
        /// <response code="500">The error code</response>
        /// <returns></returns>
        [HttpGet]
        [Route("/error")]
        public IActionResult Error()
        {
            return StatusCode(500, new Response<object>
            {
                Success = false,
                Errors = new Collection<string> { "An error occurred at the server" }
            });
        }
    }
}
