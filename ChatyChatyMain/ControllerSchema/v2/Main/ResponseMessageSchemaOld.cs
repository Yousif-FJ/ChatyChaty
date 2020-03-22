using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v2
{
    public class ResponseMessageSchemaOld
    {
        public long ID { get; set; }
        public string Sender { get; set; }
        public string Body { get; set; }
    }
}
