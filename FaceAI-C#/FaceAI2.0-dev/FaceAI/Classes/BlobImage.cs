using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAI.Classes
{
    class BlobImage
    {
        private string filename;
        private string url;

        public BlobImage(string filename, string url)
        {
            this.Filename = filename;
            this.Url = url;
        }

        public string Filename { get => filename; set => filename = value; }
        public string Url { get => url; set => url = value; }
    }
}
