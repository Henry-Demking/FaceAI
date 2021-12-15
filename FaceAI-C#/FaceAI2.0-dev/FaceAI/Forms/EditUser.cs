using FaceAI.Azure.AI;
using FaceAI.Azure.Database;
using FaceAI.Classes;
using FaceAI.Exceptions;
using FaceAI.Forms;
using FaceAI.Forms.Form_Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceAI
{
    public partial class EditUser : Form
    {
        private readonly string PATH_TO_TEMP;

        public EditUser(string tempPath)
        {
            this.PATH_TO_TEMP = tempPath;
            InitializeComponent();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            List<string> items = new List<string>();
            foreach(string item in lstItems.SelectedItems)
            {
                items.Add(item);
            }
            foreach (string item in items)
            {
                lstItems.Items.Remove(item);
                lstSelectedItems.Items.Add(item);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            List<string> items = new List<string>();
            foreach (string item in lstSelectedItems.SelectedItems)
            {
                items.Add(item);
            }
            foreach (string item in items)
            {
                lstSelectedItems.Items.Remove(item);
                lstItems.Items.Add(item);
            }
        }
    }
}
