using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Infrastructure.PictureServices;
using ChatyChaty.Infrastructure.Repositories.ChatRepository;
using ChatyChaty.Infrastructure.Repositories.MessageRepository;
using ChatyChaty.Infrastructure.Repositories.NotificationRepository;
using ChatyChaty.Infrastructure.Repositories.UserRepository;
using CloudinaryDotNet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatyChaty.Infrastructure.StartupConfiguration
{
    public static class DIExtension
    {
        public static void AddInfrastructureClasses(this IServiceCollection services)
        {
            services.AddSingleton<Cloudinary>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPictureProvider, CloudinaryPictureProvider>();
        }
    }
}
