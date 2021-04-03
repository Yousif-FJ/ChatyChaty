using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.Model.MessagingModel
{
    public class GetMessagesModel
    {
        public IEnumerable<Message> Messages { get; set; }
        public string Error { get; set; }
    }
}
