using ChatyChaty.Infrastructure.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitTest
{
    public class ChatyChatySqliteInMemoryBuilder : IDisposable
    {
        private readonly DbConnection connection;
        private readonly DbContextOptions options;

        public ChatyChatySqliteInMemoryBuilder()
        {
            options = new DbContextOptionsBuilder<ChatyChatyContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
            connection = RelationalOptionsExtension.Extract(options).Connection;
        }

        public ChatyChatyContext CreateChatyChatyContext()
        {
            var context = new ChatyChatyContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => connection.Dispose();
    }
}
