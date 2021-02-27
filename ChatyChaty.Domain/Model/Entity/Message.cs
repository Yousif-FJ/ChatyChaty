using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Model.Entity
{
    public class Message
    {
        public Message(string body, ConversationId conversationId, UserId senderId)
        {
            if (string.IsNullOrEmpty(body))
            {
                throw new ArgumentNullException(nameof(body),"Message body shouldn't be empty");
            }
            Id = new MessageId();
            TimeSent = DateTime.Now; 
            Body = body;
            ConversationId = conversationId;
            SenderId = senderId;
            Delivered = false;
        }
        public MessageId Id { get; set; }
        public string Body { get; set; }
        public ConversationId ConversationId { get; set; }
        public UserId SenderId { get; set; }
        public AppUser Sender { get; set; }
        public Conversation Conversation { get; set; }
        public bool Delivered { get; private set; }
        public DateTime TimeSent { get; set; }

        public Message MarkAsDelivered()
        {
            Delivered = true;
            return this;
        }
    }

    public record MessageId(string Value)
    {
        public MessageId() : this(Guid.NewGuid().ToString()) { }
    }
}
