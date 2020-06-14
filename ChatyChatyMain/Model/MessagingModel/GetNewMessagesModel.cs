using ChatyChaty.Model.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.MessagingModel
{
    public class GetNewMessagesModel
    {
        public IEnumerable<Message> Messages { get; set; }
        public string Error { get; set; }
    }
}
