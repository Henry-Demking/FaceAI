using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAI.Exceptions
{
    class CameraNotActiveException : Exception
    {
        public CameraNotActiveException()
        {
        }

        public CameraNotActiveException(string message)
            : base(message)
        {
        }

        public CameraNotActiveException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
