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


        /// <summary>
        /// Create an account
        /// </summary>
        /// <remarks>
        /// Try it out to check the response schema 
        /// (Account creation may be disable for security reasons)
        /// </remarks>
        /// <response code="401">Accoutn creation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody]AccountSchema accountSchema)
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


        /// <summary>
        /// Login with an existing account
        /// </summary>
        /// <remarks>
        /// Try it out to check the response schema 
        /// </remarks>
        /// <response code="401">Login failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]AccountSchema accountSchema)
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