using ChatyChaty.ControllerHubSchema.v1;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("/error")]
        public IActionResult Error()
        {
            return StatusCode(500, new ErrorResponse("An error occurred at the server"));
        }
    }
}
