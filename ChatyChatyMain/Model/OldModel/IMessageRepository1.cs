using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.OldModel
{
    public interface IMessageRepository1
    {
        IEnumerable<Message1> GetAllMessages();
        IEnumerable<Message1> GetNewMessages(long ID);
        Message1 NewMessage(Message1 message);
        IEnumerable<Message1> DeleteAllMessages();
    }
}
