using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChatyClient.Logic.AppExceptions
{
    public class AuthenticationErrorException : Exception
    {
        public AuthenticationErrorException(string message) : base(message)
        {
        }

        public AuthenticationErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
