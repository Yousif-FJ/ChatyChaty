﻿using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerHubSchema.v3
{
    public class MessageInfoReponseBase
    {
        public MessageInfoReponseBase()
        {

        }
        public MessageInfoReponseBase(Message message, long messageReceiverId)
        {
            Body = message.Body;
            ChatId = message.ConversationId;
            MessageId = message.Id;
            Sender = message.Sender.UserName;
            Delivered = message.SenderId == messageReceiverId ? message.Delivered : (bool?)null;
        }
        public long ChatId { get; set; }
        public long MessageId { get; set; }
        public string Sender { get; set; }
        public string Body { get; set; }
        public bool? Delivered { get; set; }
    }
}