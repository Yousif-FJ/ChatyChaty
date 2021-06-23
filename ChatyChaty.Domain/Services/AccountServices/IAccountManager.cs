using ChatyChaty.Domain.Model.AccountModel;
using ChatyChaty.Domain.Model.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Services.AccountServices
{
    /// <summary>
    /// Interface that handle Account and profile related logic
    /// </summary>
    public interface IAccountManager
    {
        Task<ProfileAccountModel> CreateConversationAsync(UserId senderId, string receiverUsername);
        Task<IEnumerable<ProfileAccountModel>> GetConversations(UserId userId);
        Task<ProfileAccountModel> GetConversation(ConversationId chatId, UserId userId);
        Task<string> UpdateDisplayNameAsync(UserId UserId, string NewDisplayName);
        Task<AppUser> SetPhotoAsync(UserId UserId, string fileName ,Stream file);
    }
}
