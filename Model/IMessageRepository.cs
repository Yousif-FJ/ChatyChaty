using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model
{
    public interface IMessageRepository
    {
        IEnumerable<Message> GetAllMessages();
        Message NewMessage(Message message);
    }
}
