using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Model.AccountModel
{
    public class NewConversationModel
    {
        public ProfileAccountModel Conversation { get; set; }
        public string Error { get; set; }
    }
}
