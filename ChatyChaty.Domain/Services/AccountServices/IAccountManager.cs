using ChatyChaty.Domain.Model.AccountModel;
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
        Task<NewConversationModel> NewConversationAsync(long senderId, string receiverUsername);
        Task<string> UpdateDisplayNameAsync(long UserId, string NewDisplayName);
        Task<PhotoUrlModel> SetPhotoAsync(long UserId, string fileName ,Stream file);
    }
}
