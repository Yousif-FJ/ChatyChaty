using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.ApplicationExceptions
{
    public class InvalidIdFormatException : ApplicationException
    {
        public InvalidIdFormatException(string message) : base(message)
        {
        }

        public InvalidIdFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
