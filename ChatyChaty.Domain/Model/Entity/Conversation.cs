using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Model.Entity
{
    public class Conversation
    {
        public Conversation(UserId firstUserId, UserId secondUserId)
        {
            FirstUserId = firstUserId;
            SecondUserId = secondUserId;
            Id = new ConversationId();
        }
        public ConversationId Id { get; set; }
        public AppUser FirstUser { get; set; }
        public UserId FirstUserId { get; set; }
        public AppUser SecondUser { get; set; }
        public UserId SecondUserId { get; set; }
        public ICollection<Message> Messages { get; set; }

        public UserId FindReceiverId(UserId senderId)
        {
            UserId receiverId = null;
            if (FirstUserId.Equals(senderId))
            {
                receiverId = SecondUserId;
            }
            else if (senderId.Equals(SecondUserId))
            {
                receiverId = FirstUserId;
            }
            return receiverId;
        }

        public AppUser FindReceiver(UserId senderId)
        {
            AppUser receiver = null;
            if (senderId == FirstUserId)
            {
                receiver = SecondUser;
            }
            else if (senderId == SecondUserId)
            {
                receiver = FirstUser;
            }
            return receiver;
        }
    }

    public record ConversationId(string Value)
    {
        public ConversationId()  : this(Guid.NewGuid().ToString()) { }
    }
}
