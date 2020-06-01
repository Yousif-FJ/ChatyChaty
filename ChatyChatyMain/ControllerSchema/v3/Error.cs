using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.ControllerSchema.v3
{
    public class Error
    {
        public string Field { get; set; }
        public string Description { get; set; }

        public Error(string field, string description)
        {
            Field = field; Description = description;
        }
    }
}
