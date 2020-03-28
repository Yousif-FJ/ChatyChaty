using ChatyChaty.Model;
using ChatyChaty.Model.AccountModel;
using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessageRepository;
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
        private readonly IMessageRepository messageRepository;

        public AccountManager(
            UserManager<AppUser> userManager,
            IMessageRepository messageRepository)
        {
            this.userManager = userManager;
            this.messageRepository = messageRepository;
        }

        public async Task<AppUser> GetUser(string UserName)
        {
            var user = await userManager.FindByNameAsync(UserName);
            return user;
        }

        public async Task<string> UpdateDisplayName(long UserId, string NewDisplayName)
        {
            var user = await messageRepository.GetUser(UserId);
            if (user is null)
            {
                throw new ArgumentOutOfRangeException("Invalid UserId");
            }
            var NewName = await messageRepository.UpdateDisplayName(UserId, NewDisplayName);
            return NewName;
        }
    }
}
