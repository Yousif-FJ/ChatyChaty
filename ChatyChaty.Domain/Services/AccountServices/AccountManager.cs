﻿using ChatyChaty.Domain.InfastructureInterfaces;
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

        private async Task<ProfileAccountModel> GetUserAsync(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return null;
            }
            var PhotoUrl = pictureProvider.GetPhotoURL(user.Id, user.UserName);
            return new ProfileAccountModel
            {
                Username = user.UserName,
                ChatId = user.Id,
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
            var conversation = await chatRepository.GetConversationForUsersAsync(senderId, receiver.ChatId.Value);

            await mediator.Send(new UsersGotChatUpdateAsync((senderId, conversation.Id)));

            return new NewConversationModel
            {
                Conversation = new ProfileAccountModel
                {
                    ChatId = conversation.Id,
                    DisplayName = receiver.DisplayName,
                    Username = receiver.Username,
                    PhotoURL = pictureProvider.GetPhotoURL(receiver.ChatId.Value, receiver.Username)
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
            await mediator.Send(new UsersGotChatUpdateAsync(
                userIdsGotUpdate
                .Select(u => (userId, u)).ToArray()));
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
            await mediator.Send(new UsersGotChatUpdateAsync(
                userIdsGotUpdate
                .Select(u => (userId, u)).ToArray()));
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
