using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using ChatyChaty.Infrastructure.Repositories.ChatRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XUnitTest;

namespace XUnitTest.ChatRepositoryTests
{
    public abstract class ChatRepositoryTestBase
    {
        protected readonly ChatyChatyContext Context;
        protected readonly ChatRepository ChatRepository;
        protected readonly List<AppUser> users;

        public ChatRepositoryTestBase()
        {
            Context = new ChatyChatySqliteInMemoryBuilder().CreateChatyChatyContext();
            ChatRepository = new ChatRepository(Context);
            users = new List<AppUser> { new AppUser("user1"), new AppUser("user2"), new AppUser("user3") };
            Context.Users.AddRange(users);
            Context.SaveChanges();
        }
    }
}
