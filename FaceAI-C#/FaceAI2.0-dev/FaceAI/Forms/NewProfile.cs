using FaceAI.Azure.Database;
using FaceAI.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceAI.Forms
{
    public partial class NewProfile : Form
    {
        private HomePage parent;
        private User user;
        private Database database;
        public NewProfile(HomePage parent)
        {
            this.parent = parent;
            this.user = parent.CurrentUser;
            database = new Database();
            InitializeComponent();
        }

        private void NewProfile_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.Show();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Profiles profile = new Profiles(cboSite.Text.Trim(), txtUsername.Text.Trim(), txtLink.Text.Trim());

            string query = String.Format("INSERT INTO sitelinks (username, site, link, site_username) VALUES ('{0}','{1}','{2}','{3}')", 
                user.Username, profile.Site, profile.Link, profile.Username);
            database.Insert(query);

            string message = String.Format("Profile [{0}: {1}] has been added", profile.Site, profile.Username);
            MessageBox.Show(message, "Success!", MessageBoxButtons.OK);
            this.Close();
        }
    }
}
