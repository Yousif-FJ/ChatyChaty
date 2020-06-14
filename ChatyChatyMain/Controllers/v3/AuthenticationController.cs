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
using ChatyChaty.ControllerSchema.v3;
using System.Security.Claims;

namespace ChatyChaty.Controllers.v3
{
    [SuppressAutoModelStateResponse]
    [CustomModelValidationResponse]
    [Route("api/v3/[controller]")]
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
        ///  "errors": null,
        ///  "data":{
        ///     "token": "*The Token*",
        ///     "profile": {
        ///         "username": "*UserName*",
        ///         "displayName": "*DisplayName*",
        ///         "photoURL": "*Picture URL*"
        ///         }
        ///  }
        ///}
        /// </remarks>
        /// <response code="200">Login Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody]CreateAccountSchema accountSchema)
        {
           var authenticationResult = await authenticationManager.CreateAccount(
               accountSchema.Username, accountSchema.Password, accountSchema.DisplayName);

            if (!authenticationResult.Success)
            {
                ResponseBase<AuthenticationResponseBase> authenticationSchemaF = new ResponseBase<AuthenticationResponseBase>()
                {
                    Errors = authenticationResult.Errors,
                    Success = authenticationResult.Success,
                };
                return BadRequest(authenticationSchemaF);
            }

            ProfileSchema profileSchema = new ProfileSchema
            {
                DisplayName = authenticationResult.Profile.DisplayName,
                Username = authenticationResult.Profile.Username,
                PhotoURL = authenticationResult.Profile.PhotoURL
            };

            ResponseBase<AuthenticationResponseBase> authenticationSchema = new ResponseBase<AuthenticationResponseBase>()
            {
                Success = authenticationResult.Success,
                Data = new AuthenticationResponseBase
                {
                    Token = authenticationResult.Token,
                    Profile = profileSchema
                }
            };
            return Ok(authenticationSchema);
        }


        /// <summary>
        /// Login with an existing account and retrieve JWT token with Profile
        /// </summary>
        /// <remarks>
        /// Sample Response: 
        /// {
        ///  "success": true,
        ///  "errors": null,
        ///  "data":{
        ///     "token": "*The Token*",
        ///     "profile": {
        ///         "username": "*UserName*",
        ///         "displayName": "*DisplayName*",
        ///         "photoURL": "*Picture URL*"
        ///         }
        ///  }
        ///}
        /// </remarks>
        /// <response code="200">Login Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginAccountSchema accountSchema)
        {
            var authenticationResult = await authenticationManager.Login(
                accountSchema.Username, accountSchema.Password);

            if (!authenticationResult.Success)
            {
                ResponseBase<AuthenticationResponseBase> authenticationSchemaF = new ResponseBase<AuthenticationResponseBase>()
                {
                    Errors = authenticationResult.Errors,
                    Success = authenticationResult.Success,
                };
                return BadRequest(authenticationSchemaF);
            }

            ProfileSchema profileSchema = new ProfileSchema
            {
                DisplayName = authenticationResult.Profile.DisplayName,
                Username = authenticationResult.Profile.Username,
                PhotoURL = authenticationResult.Profile.PhotoURL
            };

            ResponseBase<AuthenticationResponseBase> authenticationSchema = new ResponseBase<AuthenticationResponseBase>()
            {
                Errors = authenticationResult.Errors,
                Success = authenticationResult.Success,
                Data = new AuthenticationResponseBase
                {
                    Token = authenticationResult.Token,
                    Profile = profileSchema
                }
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
        ///  "errors": [],
        ///  "data": null
        /// }
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Password change Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [Authorize]
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordSchema passwordSchema)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var result = await authenticationManager.ChangePassword(userId, passwordSchema.CurrentPassword, passwordSchema.NewPassword);
            var changePasswordResponse = new ResponseBase<object>
            {
                Success = result.Success,
                Errors = result.Errors,
            };
            if (result.Success == false)
            {
                return BadRequest(changePasswordResponse);
            }
            else
            {
                return Ok(changePasswordResponse);
            }
        }
    }
}