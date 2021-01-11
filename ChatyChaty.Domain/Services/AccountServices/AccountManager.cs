using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Services.NotficationServices.Handler;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//TODO - Fix the notification logic

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
        private readonly IPictureProvider pictureProvider;
        private readonly ILogger<AccountManager> logger;
        private readonly IMediator mediator;

        public AccountManager(
            UserManager<AppUser> userManager,
            IUserRepository userRepository,
            IChatRepository chatRepository,
            IPictureProvider pictureProvider,
            ILogger<AccountManager> logger,
            IMediator mediator
            )
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
            this.chatRepository = chatRepository;
            this.pictureProvider = pictureProvider;
            this.logger = logger;
            this.mediator = mediator;
        }

        /// <summary>
        /// create or get a conversation between 2 users
        /// </summary>
        /// <param name="senderId">First user Id</param>
        /// <param name="receiverUsername">Second user Id</param>
        public async Task<NewConversationModel> NewConversationAsync(long senderId, string receiverUsername)
        {
            var receiver = await userManager.FindByNameAsync(receiverUsername); ;
            if (receiver == null)
            {
                return new NewConversationModel
                {
                    Error = "Requested user doesn't exist"
                };
            }

            var conversation = await chatRepository.CreateConversationAsync(senderId, receiver.Id);

            await mediator.Send(new UsersGotChatUpdateAsync((receiver.Id, conversation.Id)));

            return new NewConversationModel
            {
                Conversation = new ProfileAccountModel
                {
                    ChatId = conversation.Id,
                    DisplayName = receiver.DisplayName,
                    Username = receiver.UserName,
                    PhotoURL = await pictureProvider.GetPhotoURL(receiver.Id, receiver.UserName)
                }
            };
        }

        /// <summary>
        /// Get a list of conversations for a user
        /// </summary>
        /// <remarks>throws exception if the user doesn't exist</remarks>
        /// <param name="userId">The userId who have the conversations</param>
        /// <returns>a list of conversations</returns>
        public async Task<IEnumerable<ProfileAccountModel>> GetConversations(long userId)
        {
            var user = await userRepository.GetUserAsync(userId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException(nameof(userId),"Invalid Id");
            };

            var conversations = await chatRepository.GetConversationsWithUsersAsync(userId);

            var response = new List<ProfileAccountModel>();

            foreach (var conversation in conversations)
            {

                AppUser SecondUser = conversation.FindReceiver(user.Id);

                response.Add(new ProfileAccountModel
                {
                    ChatId = conversation.Id,
                    DisplayName = SecondUser.DisplayName,
                    Username = SecondUser.UserName,
                    PhotoURL = await pictureProvider.GetPhotoURL(SecondUser.Id, SecondUser.UserName)
                });
            }
            return response;
        }


        public async Task<ProfileAccountModel> GetConversation(long chatId, long userId)
        {
            var user = await userRepository.GetUserAsync(userId);

            var conversation = await chatRepository.GetConversationAsync(chatId);

            AppUser SecondUser = conversation.FindReceiver(user.Id);

            var response = new ProfileAccountModel
            {
                ChatId = conversation.Id,
                DisplayName = SecondUser.DisplayName,
                Username = SecondUser.UserName,
                PhotoURL = await pictureProvider.GetPhotoURL(SecondUser.Id, SecondUser.UserName)
            };
            return response;
        }

        public async Task<PhotoUrlModel> SetPhotoAsync(long userId, string fileName, Stream file)
        {
            var user = await userRepository.GetUserAsync(userId);
            if (user == null)
            {
                throw new ArgumentOutOfRangeException(nameof(userId),"Invalid userId");
            }
            var setPhotoResult = await pictureProvider.ChangePhoto(user.Id, user.UserName, fileName, file);
            await mediator.Send(new UsersUpdatedTheirProfileAsync(userId));
            return setPhotoResult;
        }

        public async Task<string> UpdateDisplayNameAsync(long userId, string newDisplayName)
        {
            var user = await userRepository.GetUserAsync(userId);
            if (user is null)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "Invalid UserId");
            }
            var newName = await userRepository.UpdateDisplayNameAsync(userId, newDisplayName);
            await mediator.Send(new UsersUpdatedTheirProfileAsync(userId));
            return newName;
        }

        //To-Do: finish implementation
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
