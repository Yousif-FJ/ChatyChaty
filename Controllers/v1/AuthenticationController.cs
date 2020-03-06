using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatyChaty.Model.AuthenticationModel;
using ChatyChaty.ControllerSchema.v1;
using ChatyChaty.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatyChaty.Controllers.v1
{
    [Route("api/v1/[controller]")]
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
        public async Task<IActionResult> CreateAccount(AccountSchema accountSchema)
        {
           var authenticationResult = await accountManager.CreateAccount(new AccountModel
           {
               UserName = accountSchema.UserName,
               Password = accountSchema.Password
           }
               );
            AuthenticationSchema authenticationSchema = new AuthenticationSchema()
            {
                Errors = authenticationResult.Errors,
                Success = authenticationResult.Success,
                Token = authenticationResult.Token
            };
            if (authenticationSchema.Success)
            {
                return Ok(authenticationSchema);
            }
            else if (!authenticationSchema.Success)
            {
                return Unauthorized(authenticationSchema);
            }
            throw new Exception("authenticationResult.Success is null");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AccountSchema accountSchema)
        {
            var authenticationResult = await accountManager.Login(new AccountModel()
            {
                UserName = accountSchema.UserName,
                Password = accountSchema.Password
            }) ;

            AuthenticationSchema authenticationSchema = new AuthenticationSchema()
            {
                Success = authenticationResult.Success,
                Errors = authenticationResult.Errors,
                Token = authenticationResult.Token
            };

            if (authenticationSchema.Success)
            {
                return Ok(authenticationSchema);
            }
            else if (!authenticationSchema.Success)
            {
                return Unauthorized(authenticationSchema);
            }
            throw new Exception("authenticationResult.Success is null");
        }
    }
}