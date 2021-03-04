using ChatyChaty.Domain.InfastructureInterfaces;
using ChatyChaty.Domain.Model.Entity;
using ChatyChaty.Infrastructure.Database;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Infrastructure.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly ChatyChatyContext dBContext;

        public UserRepository(ChatyChatyContext DBContext)
        {
            dBContext = DBContext;
        }

        public async Task<AppUser> GetAsync(UserId Id)
        {
            return await dBContext.Users.FindAsync(Id);
        }

        public async Task<AppUser> UpdateAsync(AppUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            dBContext.Users.Update(user);
            await dBContext.SaveChangesAsync();
            return user;
        }
    }
}
