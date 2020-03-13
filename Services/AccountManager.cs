using ChatyChaty.Model;
using ChatyChaty.Model.AuthenticationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public class AccountManager : IAccountManager
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;

        public AccountManager(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        public async Task<AuthenticationResult> CreateAccount(AccountModel accountModel)
        {
            if (Environment.GetEnvironmentVariable("DISABLE_REGESTRATION") == "true")
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string>() { new string("Account creation is disabled for security reasons") }
                };
            }
            AppUser identityUser = new AppUser(accountModel.UserName);
            var AccountCreationResult = await userManager.CreateAsync(identityUser, accountModel.Password);
            if (!AccountCreationResult.Succeeded)
            {
                return new AuthenticationResult 
                {
                   Success = false,
                   Errors = AccountCreationResult.Errors.Select(x => x.Description)
                };
            }

            return new AuthenticationResult
            {
                Success = true,
                Token = JwtTokenGenerator(accountModel)
            };
        }

        public async Task<AuthenticationResult> Login(AccountModel accountModel)
        {
            var user = await userManager.FindByNameAsync(accountModel.UserName);
            var LoginResult = await userManager.CheckPasswordAsync(user, accountModel.Password);
            if (!LoginResult)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string> { new string("Invalid Login cridentials") }
                };
            }
            return new AuthenticationResult
            {
                Success = true,
                Token = JwtTokenGenerator(accountModel)
            };

        }

        public async Task<AppUser> GetUser(string UserName)
        {
            var user = await userManager.FindByNameAsync(UserName);
            return user;
        }

        public async Task<string> SetPhotoID(string UserName, string PhotoName)
        {
            var user = await userManager.FindByNameAsync(UserName);
            user.PhotoID = PhotoName;
            await userManager.UpdateAsync(user);
            return PhotoName;
        }

        private string JwtTokenGenerator(AccountModel accountModel)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"));
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[]
                {
                    new Claim(type: JwtRegisteredClaimNames.UniqueName, accountModel.UserName),
                    new Claim(type: JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }
                ),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddDays(7),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Issuer"]
            };
            var token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
