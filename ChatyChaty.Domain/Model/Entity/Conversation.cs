using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatyChaty.Domain.Model.Entity
{
    public class Conversation
    {
        public Conversation(UserId firstUserId, UserId secondUserId)
        {
            FirstUserId = firstUserId ?? throw new ArgumentNullException(nameof(firstUserId));
            SecondUserId = secondUserId ?? throw new ArgumentNullException(nameof(secondUserId));
            Id = new ConversationId();
        }
        public ConversationId Id { get; private set; }
        public AppUser FirstUser { get; private set; }
        public UserId FirstUserId { get; private set; }
        public AppUser SecondUser { get; private set; }
        public UserId SecondUserId { get; private set; }
        public List<Message> Messages { get; private set; }

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

    public record ConversationId : IdBase
    {
        public ConversationId() : base() { }
        public ConversationId(string value) : base(value) { }
        override public string ToString() { return base.ToString(); }
    }
}
