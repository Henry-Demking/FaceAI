using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAI.Classes
{
    class Profiles
    {
        private string site;
        private string username;
        private string link;

        public Profiles(string site, string username, string link)
        {
            this.Site = site;
            this.Username = username;
            this.Link = link;
        }

        public string Site { get => site; set => site = value; }
        public string Username { get => username; set => username = value; }
        public string Link { get => link; set => link = value; }
    }
}
