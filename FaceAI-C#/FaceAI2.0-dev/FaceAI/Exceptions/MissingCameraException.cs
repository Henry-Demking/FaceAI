using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAI.Exceptions
{
    class MissingCameraException : Exception
    {
        public MissingCameraException()
        {
        }

        public MissingCameraException(string message)
            : base(message)
        {
        }

        public MissingCameraException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
