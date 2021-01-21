using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Model.AccountModel
{
    public class ProfileAccountModel
    {
        public long? ChatId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public long? UserId { get; set; }
        public string PhotoURL { get; set; }
    }
}
