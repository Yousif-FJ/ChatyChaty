using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Services.NotficationRequests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.AuthenticationManager
{
    /// <summary>
    /// Class that handle authentication and security related logic
    /// </summary>
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IConfiguration configuration;
        private readonly IPictureProvider pictureProvider;

        public AuthenticationManager(
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            IPictureProvider pictureProvider
            )
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.pictureProvider = pictureProvider;
        }

        public async Task<AuthenticationResult> CreateAccount(string username, string password, string displayName)
        {
            if (configuration["DISABLE_REGESTRATION"] == "true")
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new Collection<string>() { "Account Creation is disabled" }
                };
            }
            AppUser identityUser = new AppUser(username, displayName);

            var AccountCreationResult = await userManager.CreateAsync(identityUser, password);
            if (!AccountCreationResult.Succeeded)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = AccountCreationResult.Errors.Select(e => e.Description)
                };
            }
            var user = await userManager.FindByNameAsync(username);
            var profile = new ProfileAccountModel
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                PhotoURL = null
            };

            return new AuthenticationResult
            {
                Success = true,
                Token = JwtTokenGenerator(user.UserName, user.Id.ToString()),
                Profile = profile
            };
        }

        public async Task<AuthenticationResult> Login(string userName, string password)
        {
            var user = await userManager.FindByNameAsync(userName);
            var LoginResult = await userManager.CheckPasswordAsync(user, password);
            if (!LoginResult)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string> { new string("Invalid Login cridentials") }
                };
            }
            var profile = new ProfileAccountModel
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                PhotoURL = await pictureProvider.GetPhotoURL(user.Id, user.UserName)
            };
            return new AuthenticationResult
            {
                Success = true,
                Token = JwtTokenGenerator(user.UserName, user.Id.ToString()),
                Profile = profile
            };

        }


        public async Task<AuthenticationResult> ChangePassword(string Id, string currentPassword, string newPassword)
        {
            var user = await userManager.FindByIdAsync(Id);
            if (user is null)
            {
                throw new ArgumentException("Passed user doesn't exist");
            }
            var LoginResult = await userManager.CheckPasswordAsync(user, currentPassword);
            if (!LoginResult)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    Errors = new List<string> { new string("Incorrect password") }
                };
            }

            var ChangePasswordResult =
                await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return new AuthenticationResult
            {
                Success = ChangePasswordResult.Succeeded,
                Errors = ChangePasswordResult.Errors.Select(e => e.Description)
            };
        }

        private string JwtTokenGenerator(string UserName, string Id)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var secret = configuration["JWT_SECRET"];
            if (secret is null)
            {
                throw new KeyNotFoundException("Faild to obtain JWT Secret");
            }

            var key = Encoding.UTF8.GetBytes(secret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[]
                {
                    new Claim(type: JwtRegisteredClaimNames.UniqueName, UserName),
                    new Claim(type: JwtRegisteredClaimNames.NameId, Id),
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
