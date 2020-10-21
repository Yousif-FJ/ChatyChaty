using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Model.MessagingModel;
using ChatyChaty.Domain.Services.NotficationServices.Handler;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.AccountServices
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

        private async Task<ProfileAccountModel> GetUserAsync(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return null;
            }
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
        public async Task<NewConversationModel> NewConversationAsync(long senderId, string receiverUsername)
        {
            var receiver = await GetUserAsync(receiverUsername);
            if (receiver == null)
            {
                return new NewConversationModel
                {
                    Error = "Requested user doesn't exist"
                };
            }
            //get or create the conversation
            var conversation = await chatRepository.GetConversationForUsersAsync(senderId, receiver.Id.Value);

            await notificationHandler.UsersGotChatUpdateAsync((senderId, receiver.Id.Value));

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

        public async Task<PhotoUrlModel> SetPhotoAsync(long userId, string fileName, Stream file)
        {
            var user = await userRepository.GetUserAsync(userId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException("Invalid userId");
            }
            var setPhotoResult = await pictureProvider.ChangePhoto(user.Id, user.UserName, fileName, file);
            var userIdsGotUpdate = await chatRepository.GetUserContactIdsAsync(user.Id);
            await notificationHandler.UsersGotChatUpdateAsync(
                userIdsGotUpdate
                .Select(u => (userId, u)).ToArray());
            return setPhotoResult;
        }

        public async Task<string> UpdateDisplayNameAsync(long userId, string newDisplayName)
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

        public async Task<bool> DeleteAccountAsync(long userId)
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
