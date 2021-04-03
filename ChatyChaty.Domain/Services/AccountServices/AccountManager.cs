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
        public async Task<NewConversationModel> CreateConversationAsync(UserId senderId, string receiverUsername)
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
                return new NewConversationModel
                {
                    Error = "Requested user doesn't exist"
                };
            }

            var conversation = await chatRepository.GetAsync(senderId, receiver.Id);

            if (conversation is null)
            {
                conversation = new Conversation(senderId, receiver.Id);
                conversation = await chatRepository.AddAsync(conversation); 
            }

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
        public async Task<IEnumerable<ProfileAccountModel>> GetConversations(UserId userId)
        {
            var user = await userRepository.GetAsync(userId);
            if (user is null)
            {
                throw new ArgumentOutOfRangeException(nameof(userId),"Invalid Id");
            };

            var conversations = await chatRepository.GetWithUsersAsync(userId);

            var response = new List<ProfileAccountModel>();

            foreach (var conversation in conversations)
            {

                AppUser SecondUser = conversation.FindReceiver(user.Id);
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
                    PhotoURL = await pictureProvider.GetPhotoURL(SecondUser.Id, SecondUser.UserName)
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
                PhotoURL = await pictureProvider.GetPhotoURL(secondUser.Id, secondUser.UserName)
            };
            return response;
        }

        public async Task<PhotoUrlModel> SetPhotoAsync(UserId userId, string fileName, Stream file)
        {
            var user = await userRepository.GetAsync(userId);
            if (user is null)
            {
                throw new ArgumentOutOfRangeException(nameof(userId),"Invalid userId");
            }

            var setPhotoResult = await pictureProvider.ChangePhoto(user.Id, user.UserName, fileName, file);
            await mediator.Send(new UserUpdatedTheirProfileAsync(userId));
            return setPhotoResult;
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

            await mediator.Send(new UserUpdatedTheirProfileAsync(userId));

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
