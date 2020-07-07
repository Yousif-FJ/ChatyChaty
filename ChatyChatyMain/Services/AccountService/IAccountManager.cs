using ChatyChaty.Model;
using ChatyChaty.Model.AccountModel;
using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessagingModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    /// <summary>
    /// Interface that handle Account and profile related logic
    /// </summary>
    public interface IAccountManager
    {
        Task<ProfileAccountModel> GetUser(string UserName);
        Task<NewConversationModel> NewConversation(long senderId, string receiverUsername);
        Task<string> UpdateDisplayName(long UserId, string NewDisplayName);
        Task<PhotoUrlModel> SetPhoto(long UserId, IFormFile formFile);
    }
}
