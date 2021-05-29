using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatyChaty.Domain.ApplicationExceptions
{
    public class PictureProviderException : ApplicationException
    {
        public PictureProviderException(string message) : base(message)
        {
        }

        public PictureProviderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
