using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.ApplicationExceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() :base("User not found")
        {
        }
    }
}
