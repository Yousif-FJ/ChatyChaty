using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.MessageModel
{
    public interface IMessageRepository
    {
        IEnumerable<Message> GetAllMessages();
        IEnumerable<Message> GetNewMessages(long ID);
        Message NewMessage(Message message);
        IEnumerable<Message> DeleteAllMessages();
    }
}
