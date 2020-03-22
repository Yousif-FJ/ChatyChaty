using ChatyChaty.Model;
using ChatyChaty.Model.AccountModel;
using ChatyChaty.Model.DBModel;
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
        private readonly IPictureProvider pictureProvider;
        private readonly ChatyChatyContext dbcontext;

        public AccountManager(
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            IPictureProvider pictureProvider,
            ChatyChatyContext dbcontext)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.pictureProvider = pictureProvider;
            this.dbcontext = dbcontext;
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
            var profile = new Profile
            {
                DisplayName = accountModel.DisplayName,
                Username = accountModel.UserName,
                PhotoURL = pictureProvider.GetPlaceHolderURL()
            };
            
            return new AuthenticationResult
            {
                Success = true,
                Token = JwtTokenGenerator(accountModel),
                Profile = profile
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
            accountModel.Id = user.Id;
            var profile = new Profile
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                PhotoURL = await pictureProvider.GetPhotoURL(user.Id,user.UserName)
            };
            return new AuthenticationResult
            {
                Success = true,
                Token = JwtTokenGenerator(accountModel),
                Profile = profile
            };

        }

        public async Task<AppUser> GetUser(string UserName)
        {
            var user = await userManager.FindByNameAsync(UserName);
            return user;
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
                    new Claim(type:JwtRegisteredClaimNames.NameId, accountModel.Id.ToString()),
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

        public async Task<AppUser> GetUser(long UserId)
        {
            var user = await dbcontext.Users.FindAsync(UserId);
            return user;
        }
    }
}
