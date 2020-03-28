﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatyChaty.Model.AccountModel;
using ChatyChaty.ControllerSchema.v1;
using ChatyChaty.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChatyChaty.ValidationAttribute;
using Microsoft.AspNetCore.Authorization;

namespace ChatyChaty.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationManager authenticationManager;

        public AuthenticationController(IAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }


        /// <summary>
        /// [Use v2 instead] Create an account
        /// </summary>
        /// <remarks>
        /// Try it out to check the response schema 
        /// (Account creation may be disable for security reasons)
        /// </remarks>
        /// <response code="200">Login Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("CreateAccount")]
        [Obsolete("Use v2 instead")]
        public async Task<IActionResult> CreateAccount([FromBody]AccountSchemaOld accountSchema)
        {
           var authenticationResult = await authenticationManager.CreateAccount(new AccountModel
           {
               UserName = accountSchema.UserName,
               Password = accountSchema.Password
           }
               );
            AuthenticationSchemaOld authenticationSchema = new AuthenticationSchemaOld()
            {
                Errors = authenticationResult.Errors,
                Success = authenticationResult.Success,
                Token = authenticationResult.Token
            };
                return Ok(authenticationSchema);
        }


        /// <summary>
        /// [Use v2 instead] Login with an existing account
        /// </summary>
        /// <remarks>
        /// Try it out to check the response schema 
        /// </remarks>
        /// <response code="200">Login Succeed or failed</response>
        /// <response code="400">Model validation failed</response>
        /// <response code="500">Server Error (This shouldn't happen)</response>
        [HttpPost("Login")]
        [Obsolete("Use v2 instead")]
        public async Task<IActionResult> Login([FromBody]AccountSchemaOld accountSchema)
        {
            var authenticationResult = await authenticationManager.Login(new AccountModel()
            {
                UserName = accountSchema.UserName,
                Password = accountSchema.Password
            }) ;

            AuthenticationSchemaOld authenticationSchema = new AuthenticationSchemaOld()
            {
                Success = authenticationResult.Success,
                Errors = authenticationResult.Errors,
                Token = authenticationResult.Token
            };
                return Ok(authenticationSchema);
        }
    }
}