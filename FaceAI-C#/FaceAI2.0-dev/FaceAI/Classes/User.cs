using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAI.Classes
{
    class User
    {
        private readonly String username;
        private String password;
        private String first_name;
        private String surname;
        private List<string> images;
        private List<Profiles> profiles;
        private double similarity;

        public User(String username, String password ,String first_name, String surname, List<string> images = null, List<Profiles> profiles = null, double similarity = 0)
        {
            this.username = username;
            this.Password = password;
            this.First_name = first_name;
            this.Surname = surname;
            this.Images = images;
            this.Profiles = profiles;
            this.similarity = similarity;
        }

        public string Username => username;

        public string First_name { get => first_name; set => first_name = value; }
        public string Surname { get => surname; set => surname = value; }
        public string Password { get => password; set => password = value; }
        public List<string> Images { get => images; set => images = value; }
        public double Similarity { get => similarity; set => similarity = value; }
        internal List<Profiles> Profiles { get => profiles; set => profiles = value; }
    }
}
