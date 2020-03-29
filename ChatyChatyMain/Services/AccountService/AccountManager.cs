using ChatyChaty.Model;
using ChatyChaty.Model.AccountModel;
using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessageRepository;
using ChatyChaty.Model.NotficationHandler;
using Microsoft.AspNetCore.Http;
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
        private readonly INotificationHandler notificationHandler;
        private readonly IPictureProvider pictureProvider;

        public AccountManager(
            UserManager<AppUser> userManager,
            IMessageRepository messageRepository,
            INotificationHandler notificationHandler,
            IPictureProvider pictureProvider)
        {
            this.userManager = userManager;
            this.messageRepository = messageRepository;
            this.notificationHandler = notificationHandler;
            this.pictureProvider = pictureProvider;
        }

        public async Task<AppUser> GetUser(string UserName)
        {
            var user = await userManager.FindByNameAsync(UserName);
            return user;
        }

        public async Task<string> SetPhoto(long UserId, IFormFile formFile)
        {
            var user = await messageRepository.GetUser(UserId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException("Invalid userId");
            }
            await pictureProvider.ChangePhoto(user.Id, user.UserName, formFile);
            var URL = await pictureProvider.GetPhotoURL(user.Id, user.UserName);
            await notificationHandler.UserUpdatedProfile(user.Id);
            return URL;
        }

        public async Task<string> UpdateDisplayName(long UserId, string NewDisplayName)
        {
            var user = await messageRepository.GetUser(UserId);
            if (user is null)
            {
                throw new ArgumentOutOfRangeException("Invalid UserId");
            }
            var NewName = await messageRepository.UpdateDisplayName(UserId, NewDisplayName);
            await notificationHandler.UserUpdatedProfile(user.Id);
            return NewName;
        }
    }
}
