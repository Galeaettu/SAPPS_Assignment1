using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class DocumentExistsException: Exception
    {
        public DocumentExistsException() : base()
        {

        }

        public DocumentExistsException(string message) : base(message)
        {

        }

        public DocumentExistsException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
