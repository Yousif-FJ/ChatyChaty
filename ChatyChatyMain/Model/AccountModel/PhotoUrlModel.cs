using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.Model.AccountModel
{
    public class PhotoUrlModel
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string URL { get; set; }
    }
}
