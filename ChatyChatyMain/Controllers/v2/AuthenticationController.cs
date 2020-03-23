using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatyChaty.Model.AccountModel;
using ChatyChaty.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChatyChaty.ValidationAttribute;
using Microsoft.AspNetCore.Authorization;
using ChatyChaty.ControllerSchema.v2;

namespace ChatyChaty.Controllers.v2
{
    [RequireHttpsOrClose]
    [Route("api/v2/[controller]")]
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
        /// <response code="200">Login Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody]CreateAccountSchema accountSchema)
        {
           var authenticationResult = await accountManager.CreateAccount(new AccountModel
           {
               UserName = accountSchema.UserName,
               Password = accountSchema.Password,
               DisplayName = accountSchema.DisplayName
           }
               );
            ProfileSchema profileSchema = new ProfileSchema
            {
                DisplayName = authenticationResult.Profile.DisplayName,
                Username = authenticationResult.Profile.Username,
                PhotoURL = authenticationResult.Profile.PhotoURL
            };

            AuthenticationResponse authenticationSchema = new AuthenticationResponse()
            {
                Errors = authenticationResult.Errors,
                Success = authenticationResult.Success,
                Token = authenticationResult.Token,
                Profile = profileSchema
            };
                return Ok(authenticationSchema);
        }

        /// <summary>
        /// Check if the use is Authenticated
        /// </summary>
        /// <returns></returns>
        /// <response code="403">Not Authenticated</response>
        /// <response code="200">Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("IsAuthenticated")]
        [Authorize]
        public IActionResult IsAuthenticated()
        {
            return Ok();
        }


        /// <summary>
        /// Login with an existing account
        /// </summary>
        /// <remarks>
        /// Try it out to check the response schema 
        /// </remarks>
        /// <response code="200">Login Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginAccountSchema accountSchema)
        {
            var authenticationResult = await accountManager.Login(new AccountModel()
            {
                UserName = accountSchema.UserName,
                Password = accountSchema.Password
            });

            ProfileSchema profileSchema = new ProfileSchema
            {
                DisplayName = authenticationResult.Profile.DisplayName,
                Username = authenticationResult.Profile.Username,
                PhotoURL = authenticationResult.Profile.PhotoURL
            };

            AuthenticationResponse authenticationSchema = new AuthenticationResponse()
            {
                Errors = authenticationResult.Errors,
                Success = authenticationResult.Success,
                Token = authenticationResult.Token,
                Profile = profileSchema
            };
            return Ok(authenticationSchema);
        }
    }
}