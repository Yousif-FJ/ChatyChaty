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

namespace XIntegrationTest
{
    public class SqliteInMemory : IDisposable
    {
        public DbConnection Connection { get; }

        public SqliteInMemory()
        {
            var Options = new DbContextOptionsBuilder<ChatyChatyContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options;
            Connection = RelationalOptionsExtension.Extract(Options).Connection;

            new ChatyChatyContext(Options).Database.EnsureCreated();
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => Connection.Dispose();
    }
}
