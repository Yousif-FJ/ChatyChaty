using ChatyChaty.Model.AccountModel;
using ChatyChaty.Model.DBModel;
using ChatyChaty.Model.MessagingModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services.AccountServices
{
    /// <summary>
    /// Interface that handle Account and profile related logic
    /// </summary>
    public interface IAccountManager
    {
        Task<NewConversationModel> NewConversationAsync(long senderId, string receiverUsername);
        Task<string> UpdateDisplayNameAsync(long UserId, string NewDisplayName);
        Task<PhotoUrlModel> SetPhotoAsync(long UserId, IFormFile formFile);
    }
}
