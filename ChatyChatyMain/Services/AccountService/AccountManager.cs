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

        public async Task<ProfileAccountModel> GetUser(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            var PhotoUrl= await pictureProvider.GetPhotoURL(user.Id, user.UserName);
            return new ProfileAccountModel
            {
                Username = user.UserName,
                UserId = user.Id,
                DisplayName = user.DisplayName,
                PhotoURL = PhotoUrl
            };
        }

        public async Task<PhotoUrlModel> SetPhoto(long userId, IFormFile formFile)
        {
            var user = await messageRepository.GetUser(userId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException("Invalid userId");
            }
            var setPhotoResult = await pictureProvider.ChangePhoto(user.Id, user.UserName, formFile);
            await notificationHandler.UserUpdatedProfile(user.Id);
            return setPhotoResult;
        }

        public async Task<string> UpdateDisplayName(long userId, string newDisplayName)
        {
            var user = await messageRepository.GetUser(userId);
            if (user is null)
            {
                throw new ArgumentOutOfRangeException("Invalid UserId");
            }
            var newName = await messageRepository.UpdateDisplayName(userId, newDisplayName);
            await notificationHandler.UserUpdatedProfile(user.Id);
            return newName;
        }
    }
}
