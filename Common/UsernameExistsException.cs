using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class UsernameExistsException : Exception
    {
        public UsernameExistsException() : base()
        {

        }

        public UsernameExistsException(string message) : base(message)
        {

        }

        public UsernameExistsException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
