using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.DBModel
{
    public class Message
    {
        public Message(string body, long conversationId, long senderId)
        {
            Body = body;
            ConversationId = conversationId;
            SenderId = senderId;
            Delivered = false;
        }
        [Key]
        public long Id { get; set; }
        public string Body { get; set; }
        public long ConversationId { get; set; }
        public long SenderId { get; set; }
        public AppUser Sender { get; set; }
        public Conversation Conversation { get; set; }
        public bool Delivered { get; set; }

    }
}
