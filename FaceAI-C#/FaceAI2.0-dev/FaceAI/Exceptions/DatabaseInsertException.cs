using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAI.Exceptions
{
    class DatabaseInsertException : Exception
    {
        public DatabaseInsertException()
        {
        }

        public DatabaseInsertException(string message)
            : base(message)
        {
        }

        public DatabaseInsertException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
