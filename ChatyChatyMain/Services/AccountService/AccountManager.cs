using ChatyChaty.Model;
using ChatyChaty.Model.AccountModel;
using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessageRepository;
using ChatyChaty.Model.MessagingModel;
using ChatyChaty.Model.NotficationHandler;
using Google.Apis.Upload;
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

namespace ChatyChaty.Services
{
    public class AccountManager : IAccountManager
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IMessageRepository messageRepository;
        private readonly INotificationHandler notificationHandler;
        private readonly IPictureProvider pictureProvider;
        private readonly ILogger<AccountManager> logger;

        public AccountManager(
            UserManager<AppUser> userManager,
            IMessageRepository messageRepository,
            INotificationHandler notificationHandler,
            IPictureProvider pictureProvider,
            ILogger<AccountManager> logger
            )
        {
            this.userManager = userManager;
            this.messageRepository = messageRepository;
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
            var senderDB = await messageRepository.GetUserAsync(senderId);
            var reciverDB = await GetUser(receiverUsername);
            if (senderDB == null)
            {
                //throw exception because userId should come from trusted source (authentication header)
                throw new ArgumentOutOfRangeException("Invalid sender IDs");
            }
            if (reciverDB == null)
            {
                return new NewConversationModel
                {
                    Error = "Requested user doesn't exist"
                };
            }
            //get or create the conversation
            var conversation = await messageRepository.FindConversationForUsersAsync(senderDB.Id, reciverDB.Id.Value);
            if (conversation == null)
            {
                conversation = await messageRepository.CreateConversationForUsersAsync(senderDB.Id, reciverDB.Id.Value);
            }

            await notificationHandler.UserGotChatUpdate(reciverDB.Id.Value);

            return new NewConversationModel
            {
                Conversation = new ConversationInfo
                {
                    ConversationId = conversation.Id,
                    SecondUserId = reciverDB.Id.Value,
                    SecondUserDisplayName = reciverDB.DisplayName,
                    SecondUserUsername = reciverDB.Username,
                    SecondUserPhoto = await pictureProvider.GetPhotoURL(reciverDB.Id.Value, reciverDB.Username)
                }
            };
        }

        public async Task<PhotoUrlModel> SetPhoto(long userId, IFormFile formFile)
        {
            var user = await messageRepository.GetUserAsync(userId);
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
            var user = await messageRepository.GetUserAsync(userId);
            if (user is null)
            {
                throw new ArgumentOutOfRangeException("Invalid UserId");
            }
            var newName = await messageRepository.UpdateDisplayNameAsync(userId, newDisplayName);
            await notificationHandler.UserUpdatedProfile(user.Id);
            return newName;
        }

        public async Task<bool> DeleteAccount(long userId) 
        {
            var user = await messageRepository.GetUserAsync(userId);
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
