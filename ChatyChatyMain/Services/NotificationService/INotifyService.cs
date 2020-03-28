using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Services
{
    public interface INotifyService
    {
        void UserUpdateProfile(long UserId);
        void NotifiyUserForMessage(long UserId);
        bool CheckForUpdates(long UserId);
    }
}
