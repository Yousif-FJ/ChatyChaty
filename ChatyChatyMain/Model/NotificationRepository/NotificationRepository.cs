using ChatyChaty.Model.DBModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChatyChaty.Model.NotificationRepository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ChatyChatyContext dbContext;

        public NotificationRepository(ChatyChatyContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public async Task<Notification> CheckForUpdatesAsync(long userId)
        {
            var Notification = await dbContext.Notifications.FirstOrDefaultAsync(n => n.UserId == userId);
            if (Notification == null)
            {
                Notification = await IntializeNotificationHandlerAsync(userId);
            }
            var NotificationResponsee = new Notification
            {
                ChatUpdate = Notification.ChatUpdate,
                MessageUpdate = Notification.MessageUpdate,
                Id = Notification.Id
            };

            Notification.ChatUpdate = false;
            Notification.MessageUpdate = false;
            Notification.DeliveredUpdate = false;
            dbContext.Notifications.Update(Notification);
            await dbContext.SaveChangesAsync();
            return NotificationResponsee;
        }

        public async Task<Notification> IntializeNotificationHandlerAsync(long userId)
        {
            var notification = await dbContext.Notifications.FirstOrDefaultAsync(n => n.UserId == userId);
            if (notification == null)
            {
                notification = new Notification
                {
                    UserId = userId,
                    ChatUpdate = false,
                    MessageUpdate = false,
                    DeliveredUpdate = false
                };
                notification = (await dbContext.Notifications.AddAsync(notification)).Entity;
            }
            else
            {
                notification.MessageUpdate = false;
                notification.ChatUpdate = false;
                notification.DeliveredUpdate = false;
                notification = dbContext.Notifications.Update(notification).Entity;
            }
            await dbContext.SaveChangesAsync();
            return notification;
        }

        public async Task UsersGotChatUpdateAsync(params long[] userIds)
        {
            var notifications = await dbContext.Notifications.Where(u => userIds.Contains(u.Id)).ToListAsync();
            foreach (var notification in notifications)
            {
                notification.ChatUpdate = true;
            }
            dbContext.Notifications.UpdateRange(notifications);
            await dbContext.SaveChangesAsync();
        }

        public async Task UsersGotMessageDeliveredAsync(params long[] userIds)
        {
            var notifications = await dbContext.Notifications.Where(u => userIds.Contains(u.Id)).ToListAsync();
            foreach (var notification in notifications)
            {
                notification.DeliveredUpdate = true;
            }
            dbContext.Notifications.UpdateRange(notifications);
            await dbContext.SaveChangesAsync();
        }

        public async Task UserGotNewMessageAsync(long userId)
        {
            var notification = await dbContext.Notifications.FirstOrDefaultAsync(n => n.UserId == userId);
            if (notification != null)
            {
                notification.ChatUpdate = true;
                dbContext.Notifications.Update(notification);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
