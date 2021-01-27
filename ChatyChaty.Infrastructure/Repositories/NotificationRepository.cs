using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChatyChaty.Infrastructure.Repositories.NotificationRepository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ChatyChatyContext dbContext;

        public NotificationRepository(ChatyChatyContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Notification> GetNotificationAsync(long userId)
        {
            var notification = await dbContext.Notifications.FirstOrDefaultAsync(n => n.UserId == userId);
            if (notification == null)
            {
                notification = (await dbContext.Notifications.AddAsync(new Notification(userId))).Entity;
            }
            await dbContext.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification> ResetUpdatesAsync(long userId)
        {
            var notification = await dbContext.Notifications.FirstOrDefaultAsync(n => n.UserId == userId);
            if (notification == null)
            {
                notification = (await dbContext.Notifications.AddAsync(new Notification(userId))).Entity;
            }
            notification.Reset();
            await dbContext.SaveChangesAsync();
            return notification;
        }

        public async Task StoreUsersChatUpdateStatusAsync(params long[] userIds)
        {
            await UserGotUpdateCommon(notification => { notification.GotChatUpdate(); }, userIds);
        }

        public async Task StoreUsersMessageStatusAsync(params long[] userIds)
        {
            await UserGotUpdateCommon(notification => { notification.GotDeliveredUpdate(); }, userIds);
        }

        public async Task StoreUserNewMessageStatusAsync(long userId)
        {
            await UserGotUpdateCommon(notification => { notification.GotMessageUpdate(); }, userId);
        }

        private async Task UserGotUpdateCommon(Action<Notification> updateMethod, params long[] userIds)
        {
            var usersAndNotification = await dbContext.Users
             .Include(user => user.Notification)
             .Where(user => userIds.Contains(user.Id))
             .Select(user => new { user.Id, user.Notification })
             .ToListAsync();

            var notifications = new List<Notification>();
            foreach (var item in usersAndNotification)
            {
                if (item.Notification == null)
                {
                    var notification = new Notification(item.Id);
                    updateMethod(notification);
                    notifications.Add(notification);
                }
                else
                {
                    updateMethod(item.Notification);
                    notifications.Add(item.Notification);
                }
            }
            dbContext.Notifications.UpdateRange(notifications);
            await dbContext.SaveChangesAsync();
        }
    }
}
