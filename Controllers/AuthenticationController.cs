using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatyChaty.Model.AccountModels;
using ChatyChaty.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatyChaty.Controllers
{
    [Route("api/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAccountManager accountManager;

        public AuthenticationController(IAccountManager accountManager)
        {
            this.accountManager = accountManager;
        }

        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount(AccountModel accountModel)
        {
           var authenticationResult = await accountManager.CreateAccount(accountModel);
            if (authenticationResult.Success)
            {
                return Ok(authenticationResult);
            }
            else if (!authenticationResult.Success)
            {
                return Unauthorized(authenticationResult);
            }
            throw new Exception("authenticationResult.Success is null");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AccountModel accountModel)
        {
            var authenticationResult = await accountManager.Login(accountModel);
            if (authenticationResult.Success)
            {
                return Ok(authenticationResult);
            }
            else if (!authenticationResult.Success)
            {
                return Unauthorized(authenticationResult);
            }
            throw new Exception("authenticationResult.Success is null");
        }
    }
}