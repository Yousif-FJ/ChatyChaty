using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Infrastructure.PictureServices;
using ChatyChaty.Infrastructure.Repositories.ChatRepository;
using ChatyChaty.Infrastructure.Repositories.MessageRepository;
using ChatyChaty.Infrastructure.Repositories.UserRepository;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatyChaty.Infrastructure.StartupConfiguration
{
    public static class DIExtension
    {
        public static void AddInfrastructureClasses(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddSingleton<Cloudinary>(new Cloudinary(Configuration["CLOUDINARY_URL"]));
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPictureProvider, CloudinaryPictureProvider>();
        }
    }
}
