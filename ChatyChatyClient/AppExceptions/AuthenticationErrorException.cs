using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.AppExceptions
{
    public class AuthenticationErrorException : ApplicationException
    {
        public AuthenticationErrorException(string message) : base(message)
        {
        }

        public AuthenticationErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
