using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.Entities
{
    public class Message : IComparable<Message>
    {
        public Message(string body, string senderName, string id, bool? isDelivered, DateTime sentTime, DateTime? statusUpdateTime)
        {
            Body = body ?? throw new ArgumentNullException(nameof(body));
            SenderName = senderName ?? throw new ArgumentNullException(nameof(senderName));
            Id = id ?? throw new ArgumentNullException(nameof(id));
            IsDelivered = isDelivered;
            SentTime = sentTime;
            StatusUpdateTime = statusUpdateTime;
        }

        public string Body { get; init; }
        public string SenderName { get; init; }
        public string Id { get; init; }
        public bool? IsDelivered { get; set; }
        public DateTime SentTime { get; set; }
        public DateTime? StatusUpdateTime { get; set; }

        public int CompareTo(Message other)
        {
            if (this.SentTime > other.SentTime)
            {
                return 1;
            }
            else if (this.SentTime < other.SentTime)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
