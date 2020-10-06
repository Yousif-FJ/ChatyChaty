﻿using ChatyChaty.Model;
using ChatyChaty.Model.AccountModel;
using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessagingModel;
using ChatyChaty.Model.Repositories.ChatRepository;
using ChatyChaty.Model.Repositories.UserRepository;
using ChatyChaty.Services.NotificationServices;
using ChatyChaty.Services.PictureServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.Services.AccountServices
{
    /// <summary>
    /// Class that handle Account and profile related logic
    /// </summary>
    public class AccountManager : IAccountManager
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IUserRepository userRepository;
        private readonly IChatRepository chatRepository;
        private readonly INotificationHandler notificationHandler;
        private readonly IPictureProvider pictureProvider;
        private readonly ILogger<AccountManager> logger;

        public AccountManager(
            UserManager<AppUser> userManager,
            IUserRepository userRepository,
            IChatRepository chatRepository,
            INotificationHandler notificationHandler,
            IPictureProvider pictureProvider,
            ILogger<AccountManager> logger
            )
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
            this.chatRepository = chatRepository;
            this.notificationHandler = notificationHandler;
            this.pictureProvider = pictureProvider;
            this.logger = logger;
        }

        public async Task<ProfileAccountModel> GetUser(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            var PhotoUrl = await pictureProvider.GetPhotoURL(user.Id, user.UserName);
            return new ProfileAccountModel
            {
                Username = user.UserName,
                Id = user.Id,
                DisplayName = user.DisplayName,
                PhotoURL = PhotoUrl
            };
        }

        /// <summary>
        /// create or get a conversation between 2 users
        /// </summary>
        /// <param name="senderId">First user Id</param>
        /// <param name="receiverUsername">Second user Id</param>
        /// <returns>A long that represent the created conversation Id</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">thrown when sender users don't exist</exception>
        public async Task<NewConversationModel> NewConversation(long senderId, string receiverUsername)
        {
            var sender = await userRepository.GetUserAsync(senderId);
            var receiver = await GetUser(receiverUsername);
            if (sender == null)
            {
                //throw exception because userId should come from trusted source (authentication header)
                throw new ArgumentOutOfRangeException("Invalid sender IDs");
            }
            if (receiver == null)
            {
                return new NewConversationModel
                {
                    Error = "Requested user doesn't exist"
                };
            }
            //get or create the conversation
            var conversation = await chatRepository.GetConversationForUsersAsync(sender.Id, receiver.Id.Value);

            await notificationHandler.UsersGotChatUpdateAsync((senderId,receiver.Id.Value));

            return new NewConversationModel
            {
                Conversation = new ConversationInfo
                {
                    ConversationId = conversation.Id,
                    SecondUserId = receiver.Id.Value,
                    SecondUserDisplayName = receiver.DisplayName,
                    SecondUserUsername = receiver.Username,
                    SecondUserPhoto = await pictureProvider.GetPhotoURL(receiver.Id.Value, receiver.Username)
                }
            };
        }

        public async Task<PhotoUrlModel> SetPhoto(long userId, IFormFile formFile)
        {
            var user = await userRepository.GetUserAsync(userId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException("Invalid userId");
            }
            var setPhotoResult = await pictureProvider.ChangePhoto(user.Id, user.UserName, formFile);
            var userIdsGotUpdate = await chatRepository.GetUserContactIdsAsync(user.Id);
            await notificationHandler.UsersGotChatUpdateAsync(
                userIdsGotUpdate
                .Select(u => (userId, u)).ToArray());
            return setPhotoResult;
        }

        public async Task<string> UpdateDisplayName(long userId, string newDisplayName)
        {
            var user = await userRepository.GetUserAsync(userId);
            if (user is null)
            {
                throw new ArgumentOutOfRangeException("Invalid UserId");
            }
            var newName = await userRepository.UpdateDisplayNameAsync(userId, newDisplayName);
            var userIdsGotUpdate = await chatRepository.GetUserContactIdsAsync(user.Id);
            await notificationHandler.UsersGotChatUpdateAsync(
                userIdsGotUpdate
                .Select(u => (userId, u)).ToArray());
            return newName;
        }

        public async Task<bool> DeleteAccount(long userId) 
        {
            var user = await userRepository.GetUserAsync(userId);
            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return result.Succeeded;
            }
            logger.LogWarning($"Account deletion failed with the errors : {string.Join(",", result.Errors.Select(e => e.Description))}");
            return false;
        }

    }
}
