using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAI.Exceptions
{
    class NoFaceException : Exception
    {
        public NoFaceException()
        {
        }

        public NoFaceException(string message)
            : base(message)
        {
        }

        public NoFaceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
