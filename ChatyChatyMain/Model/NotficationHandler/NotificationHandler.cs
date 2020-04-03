using ChatyChaty.Model.DBModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.NotficationHandler
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly ChatyChatyContext dbContext;

        public NotificationHandler(ChatyChatyContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Notification> CheckForUpdates(long UserId)
        {
            var NotificationUpdate = await dbContext.Notifications.FindAsync(UserId);
            var NotificationResponsee = new Notification
            {
                ChatUpdate = NotificationUpdate.ChatUpdate,
                MessageUpdate = NotificationUpdate.MessageUpdate,
                Id = NotificationUpdate.Id
            };
            NotificationUpdate.ChatUpdate = false;
            NotificationUpdate.MessageUpdate = false;
            NotificationUpdate.DeliveredUpdate = false;
            dbContext.Notifications.Update(NotificationUpdate);
            await dbContext.SaveChangesAsync();
            return NotificationResponsee;
        }

        public async Task IntializeNofificationHandler(long UserId)
        {
            var user = await dbContext.Users.Include(u => u.Notification).FirstOrDefaultAsync(u => u.Id == UserId);
            if (user.Notification == null)
            {
                user.Notification = new Notification
                {
                    ChatUpdate = false,
                    MessageUpdate = false,
                    DeliveredUpdate = false
                };
            }
            else
            {
                user.Notification.MessageUpdate = false;
                user.Notification.ChatUpdate = false;
                user.Notification.DeliveredUpdate = false;
            }
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
        }

        public async Task NotifySenderMessagesWhereDelivered(IEnumerable<long> SenderIds)
        {
            var notifications = await dbContext.Notifications.Where(u => SenderIds.Contains(u.Id)).ToListAsync();
            foreach (var notification in notifications)
            {
                notification.DeliveredUpdate = true;
            }
            dbContext.Notifications.UpdateRange(notifications);
            await dbContext.SaveChangesAsync();
        }



        public async Task UserGotChatUpdate(long UserId)
        {
            var notitication = await dbContext.Notifications.FindAsync(UserId);
            if (notitication == null)
            {
                await IntializeNofificationHandler(UserId);
                notitication = await dbContext.Notifications.FindAsync(UserId);
            }
            notitication.ChatUpdate = true;
            dbContext.Notifications.Update(notitication);
            await dbContext.SaveChangesAsync();
        }

        public async Task UserGotNewMessage(long UserId)
        {
            var notitication = await dbContext.Notifications.FindAsync(UserId);
            if (notitication == null)
            {
                await IntializeNofificationHandler(UserId);
                notitication = await dbContext.Notifications.FindAsync(UserId);
            }
            notitication.MessageUpdate = true;
            dbContext.Notifications.Update(notitication);
            await dbContext.SaveChangesAsync();
        }

        public async Task UserUpdatedProfile(long UserId)
        {
            var conversations = await dbContext.Conversations
                .Where(c => (c.FirstUserId == UserId || c.SecondUserId == UserId))
                .ToListAsync();

            foreach (var conversation in conversations)
            {
                if (conversation.FirstUserId == UserId)
                {
                    await UserGotChatUpdate(conversation.SecondUserId);
                }
                else if (conversation.SecondUserId == UserId)
                {
                    await UserGotChatUpdate(conversation.FirstUserId);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Conversatoin is not for the user");
                }
            }
        }  
    }
}
