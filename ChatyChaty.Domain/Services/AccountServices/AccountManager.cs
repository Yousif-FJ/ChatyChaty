using ChatyChaty.Domain.ApplicationExceptions;
using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Domain.Services.NotficationRequests;
using ChatyChaty.Domain.Services.ScopeServices;
using MediatR;
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
        //TODO- Factor out userManager and make it integrated with user repo
        private readonly UserManager<AppUser> userManager;
        private readonly IUserRepository userRepository;
        private readonly IChatRepository chatRepository;
        private readonly IPictureProvider pictureProvider;
        private readonly ILogger<AccountManager> logger;
        private readonly IFireAndForgetService fireAndForgetService;

        public AccountManager(
            UserManager<AppUser> userManager,
            IUserRepository userRepository,
            IChatRepository chatRepository,
            IPictureProvider pictureProvider,
            ILogger<AccountManager> logger,
            IFireAndForgetService fireAndForgetService
            )
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
            this.chatRepository = chatRepository;
            this.pictureProvider = pictureProvider;
            this.logger = logger;
            this.fireAndForgetService = fireAndForgetService;
        }

        /// <summary>
        /// create or get a conversation between 2 users
        /// </summary>
        /// <param name="senderId">First user Id</param>
        /// <param name="receiverUsername">Second user Id</param>
        public async Task<ProfileAccountModel> CreateConversationAsync(UserId senderId, string receiverUsername)
        {
            if (senderId is null)
            {
                throw new ArgumentNullException(nameof(senderId));
            }

            if (string.IsNullOrWhiteSpace(receiverUsername))
            {
                throw new ArgumentException($"'{nameof(receiverUsername)}' cannot be null or whitespace", nameof(receiverUsername));
            }

            var sender = await userRepository.GetAsync(senderId);
            if (sender is null)
            {
                throw new ArgumentOutOfRangeException(nameof(senderId), $"{senderId} is invalid");
            }

            var receiver = await userManager.FindByNameAsync(receiverUsername); 
            if (receiver is null)
            {
                throw new UserNotFoundException();
            }

            var conversation = await chatRepository.GetAsync(senderId, receiver.Id);

            if (conversation is null)
            {
                conversation = new Conversation(senderId, receiver.Id);
                conversation = await chatRepository.AddAsync(conversation); 
            }

            fireAndForgetService.RunActionWithoutWaitingAsync<IMediator>(mediator
                => mediator.Send(new UsersGotChatUpdateAsync((receiver.Id, conversation.Id))));

            return new ProfileAccountModel
            {
                ChatId = conversation.Id,
                DisplayName = receiver.DisplayName,
                Username = receiver.UserName,
                PhotoURL = receiver.PhotoURL
            };
        }

        /// <summary>
        /// Get a list of conversations for a user
        /// </summary>
        /// <remarks>throws exception if the user doesn't exist</remarks>
        /// <param name="userId">The userId who have the conversations</param>
        /// <returns>a list of conversations</returns>
        public async Task<IEnumerable<ProfileAccountModel>> GetConversations(UserId userId)
        {
            var conversations = await chatRepository.GetWithUsersAsync(userId);

            var response = new List<ProfileAccountModel>();

            foreach (var conversation in conversations)
            {

                AppUser SecondUser = conversation.FindReceiver(userId);
                if (SecondUser is null)
                {
                    throw new InvalidOperationException("Conversation is not for the given user");
                }

                response.Add(new ProfileAccountModel
                {
                    ChatId = conversation.Id,
                    DisplayName = SecondUser.DisplayName,
                    Username = SecondUser.UserName,
                    UserId = SecondUser.Id,
                    PhotoURL = SecondUser.PhotoURL
                });
            }
            return response;
        }


        public async Task<ProfileAccountModel> GetConversation(ConversationId chatId, UserId userId)
        {
            var conversation = await chatRepository.GetAsync(chatId);

            var secondUserId = conversation.FindReceiverId(userId);

            if (secondUserId is null)
            {
                throw new InvalidOperationException("Conversation is not for the given user");
            }

            var secondUser = await userRepository.GetAsync(secondUserId);


            var response = new ProfileAccountModel
            {
                ChatId = conversation.Id,
                DisplayName = secondUser.DisplayName,
                Username = secondUser.UserName,
                PhotoURL = secondUser.PhotoURL
            };
            return response;
        }

        public async Task<AppUser> SetPhotoAsync(UserId userId, string fileName, Stream file)
        {
            var user = await userRepository.GetAsync(userId);
            if (user is null)
            {
                throw new ArgumentOutOfRangeException(nameof(userId),"Invalid userId");
            }

            var photoUrl = await pictureProvider.ChangePhoto(user.Id, fileName, file);

            if (photoUrl is null)
            {
                throw new PictureProviderException("picture provider returned null");
            }
            user.ChangePhotoUrl(photoUrl);

            await userRepository.UpdateAsync(user);

            fireAndForgetService.RunActionWithoutWaitingAsync<IMediator>(mediator => mediator.Send(new UserUpdatedTheirProfileAsync(userId)));
             
            return user;
        }

        public async Task<string> UpdateDisplayNameAsync(UserId userId, string newDisplayName)
        {
            if (string.IsNullOrWhiteSpace(newDisplayName))
            {
                throw new ArgumentException($"'{nameof(newDisplayName)}' cannot be null or whitespace", nameof(newDisplayName));
            }

            var user = await userRepository.GetAsync(userId);
            if (user is null)
            {
                throw new ArgumentOutOfRangeException(nameof(userId), "Invalid UserId");
            }
            user.ChangeDisplayName(newDisplayName);

            await userRepository.UpdateAsync(user);

            fireAndForgetService.RunActionWithoutWaitingAsync<IMediator>(mediator => mediator.Send(new UserUpdatedTheirProfileAsync(userId)));

            return user.DisplayName;
        }

        //To-Do: finish implementation
        public async Task<bool> DeleteAccountAsync(UserId userId)
        {
            var user = await userRepository.GetAsync(userId);
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
