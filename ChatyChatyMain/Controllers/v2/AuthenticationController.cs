using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChatyChaty.ControllerHubSchema.v2;
using System.Security.Claims;
using ChatyChaty.Services.AuthenticationManager;

namespace ChatyChaty.Controllers.v2
{
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v2/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationManager authenticationManager;

        public AuthenticationController(IAuthenticationManager accountManager)
        {
            this.authenticationManager = accountManager;
        }


        /// <summary>
        /// Create an account and retrieve JWT token with Profile 
        /// </summary>
        /// <remarks>
        /// Sample Response: 
        /// {
        ///  "success": true,
        ///  "token": "*The Token*",
        ///  "errors": null,
        ///  "profile": {
        /// "username": "*UserName*",
        ///    "displayName": "*DisplayName*",
        ///    "photoURL": "*Picture URL*"
        ///  }
        ///}
        /// </remarks>
        /// <response code="200">Login Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("CreateAccount")]
        [Obsolete]
        public async Task<IActionResult> CreateAccount([FromBody]CreateAccountSchema accountSchema)
        {
           var authenticationResult = await authenticationManager.CreateAccount(
               accountSchema.UserName, accountSchema.Password, accountSchema.DisplayName);

            if (!authenticationResult.Success)
            {
                AuthenticationResponse authenticationSchemaF = new AuthenticationResponse()
                {
                    Errors = authenticationResult.Errors,
                    Success = authenticationResult.Success,
                };
                return Ok(authenticationSchemaF);
            }

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
        /// Check if the use is Authenticated And return the logged in user (For testing)
        /// </summary>
        /// <returns></returns>
        /// <response code="401">Not Authenticated</response>
        /// <response code="200">Authenticated</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpGet("IsAuthenticated")]
        [Authorize]
        [Obsolete]
        public IActionResult IsAuthenticated()
        {
            var UserNameClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            return Ok($"Current Active User Is {UserNameClaim.Value}");
        }


        /// <summary>
        /// Login with an existing account and retrieve JWT token with Profile
        /// </summary>
        /// <remarks>
        /// Sample Response: 
        /// {
        ///  "success": true,
        ///  "token": "*The Token*",
        ///  "errors": null,
        ///  "profile": {
        ///   "username": "*UserName*",
        ///   "displayName": "*DisplayName*",
        ///   "photoURL": "*Picture URL*"
        ///  }
        ///}
        /// </remarks>
        /// <response code="200">Login Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("Login")]
        [Obsolete]
        public async Task<IActionResult> Login([FromBody]LoginAccountSchema accountSchema)
        {
            var authenticationResult = await authenticationManager.Login(
                accountSchema.UserName, accountSchema.Password);

            if (!authenticationResult.Success)
            {
                AuthenticationResponse authenticationSchemaF = new AuthenticationResponse()
                {
                    Errors = authenticationResult.Errors,
                    Success = authenticationResult.Success,
                };
                return Ok(authenticationSchemaF);
            }


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
        /// Change the logged in user password
        /// </summary>
        /// <remarks>
        /// <br>Currently this doesn't make existing logins sessions invalid. </br>
        /// <br>If you set the new password back to the same current password you won't get any errors</br>
        /// Example response: 
        /// {
        ///  "success": true,
        ///  "errors": []
        /// }
        /// </remarks>
        /// <param name="passwordSchema"></param>
        /// <returns></returns>
        /// <response code="200">Password change Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [Obsolete]
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordSchema passwordSchema)
        {
            var UserId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var result = await authenticationManager.ChangePassword(UserId, passwordSchema.CurrentPassword, passwordSchema.NewPassword);
            ChangePasswordResponse changePasswordResponse = new ChangePasswordResponse
            {
                Success = result.Success,
                Errors = result.Errors
            };
            return Ok(changePasswordResponse);
        }
    }
}