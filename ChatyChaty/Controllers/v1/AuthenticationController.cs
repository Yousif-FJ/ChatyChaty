using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ChatyChaty.ValidationAttribute;
using ChatyChaty.ControllerHubSchema.v1;
using ChatyChaty.Domain.Services.AuthenticationManager;

namespace ChatyChaty.Controllers.v1
{
    [ApiExplorerSettings(GroupName = "v1")]
    [SuppressAutoModelStateResponse]
    [CustomModelValidationResponse]
    [Route("api/v1/[controller]")]
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
        /// <br>Example Response: </br>
        /// <br>
        /// {
        /// "token": "*The Token*",
        /// "profile": {
        ///     "username": "*UserName*",
        ///     "displayName": "*DisplayName*",
        ///     "photoURL": "*Picture URL*"
        ///     }
        ///  }
        /// </br>
        /// </remarks>
        /// <response code="200">Login Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("NewAccount")]
        public async Task<IActionResult> CreateAccount([FromBody]CreateAccountSchema accountSchema)
        {
           var result = await authenticationManager.CreateAccount(
               accountSchema.Username, accountSchema.Password, accountSchema.DisplayName);

            if (result.Success == false)
            {
                return BadRequest(new ErrorResponse(result.Errors));
            }

            ProfileResponse profile = new ProfileResponse
            {
                DisplayName = result.Profile.DisplayName,
                Username = result.Profile.Username,
                PhotoURL = result.Profile.PhotoURL
            };

            var response = new AuthResponse
            {
                Token = result.Token,
                Profile = profile
            };
            return Ok(response);
        }


        /// <summary>
        /// Login with an existing account and retrieve JWT token with Profile
        /// </summary>
        /// <remarks>
        /// <br>Example Response: </br>
        /// <br>
        /// {
        /// "token": "*The Token*",
        /// "profile": {
        ///     "username": "*UserName*",
        ///     "displayName": "*DisplayName*",
        ///     "photoURL": "*Picture URL*"
        ///     }
        ///  }
        /// </br>
        /// </remarks>
        /// <response code="200">Login Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("Account")]
        public async Task<IActionResult> Login([FromBody]LoginAccountSchema accountSchema)
        {
            var authenticationResult = await authenticationManager.Login(
                accountSchema.Username, accountSchema.Password);

            if (authenticationResult.Success == false)
            {
                return BadRequest(new ErrorResponse(authenticationResult.Errors));
            }

            ProfileResponse profile = new ProfileResponse
            {
                DisplayName = authenticationResult.Profile.DisplayName,
                Username = authenticationResult.Profile.Username,
                PhotoURL = authenticationResult.Profile.PhotoURL
            };

            var response = new AuthResponse
            {
                Token = authenticationResult.Token,
                Profile = profile
            };

            return Ok(response);
        }


        /// <summary>
        /// Change the logged in user password
        /// </summary>
        /// <remarks>
        /// <br>Currently this doesn't make existing logins sessions invalid. </br>
        /// <br>If you set the new password back to the same current password you won't get any errors</br>
        /// <br>Return empty response</br>
        /// </remarks>
        /// <response code="200">Password change Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpPatch("Password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordSchema passwordSchema)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var result = await authenticationManager.ChangePassword(userId, passwordSchema.CurrentPassword, passwordSchema.NewPassword);

            if (result.Success == false)
            {
                return BadRequest(new ErrorResponse(result.Errors));
            }
            else
            {
                return Ok();
            }
        }
    }
}