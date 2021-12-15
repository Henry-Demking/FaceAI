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
using Console = FaceAI.Classes.Console;

namespace FaceAI
{
    public partial class HomePage : Form
    {
        private readonly string PATH_TO_TEMP;
        private User currentUser;
        private Database dbs;
        private Bitmap userImage;
        private Bitmap compareImage;
        private RecognitionActions recognitionModel;
        private List<string> tempFaces;
        private List<List<User>> foundUsers;
        private Console console;
        private List<Face> cache;

        internal User CurrentUser { get => currentUser;}

        public HomePage(string tempPath)
        {
            this.PATH_TO_TEMP = tempPath;
            dbs = new Database();
            InitializeComponent();
            cache = new List<Face>();
            console = new Console(lstConsole);
            recognitionModel = new RecognitionActions(PATH_TO_TEMP, console, cache);

            pctCompare.SizeMode = PictureBoxSizeMode.Zoom;
            tempFaces = new List<string>();
            foundUsers = new List<List<User>>();
            tabControl1.TabPages.Clear();
            

            tabControl1.MouseClick += new MouseEventHandler(tabMouseClick);
            console.Out("> Setup Complete");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Allow the user to sign up by opening a form and hiding this one
            SignUp signup = new SignUp(this, PATH_TO_TEMP);

            signup.Show();
            this.Hide();
        }

        private void HomePage_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Delete the temp path
            console.Out("> Closing");
            console.Out("...");
            try
            {
                userImage = null;
                currentUser = null;
                if (pctUser.Image != null) { pctUser.Image.Dispose(); }
                foundUsers = null;
                DirectoryInfo d = new DirectoryInfo(PATH_TO_TEMP);
                foreach (var file in d.GetFiles("*.*"))
                {
                    File.Delete($"{PATH_TO_TEMP}{file.FullName}");
                }

                Directory.Delete(PATH_TO_TEMP, true);
            }
            catch (Exception)
            {
            }
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            // Get username and Password
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Try to log the user in
            try
            {
                console.Out($"> Attempting login with username: {username}");
                currentUser = dbs.GetUser(username, password);
                currentUser.Images = dbs.GetImageFiles(username);
                setFace(currentUser.Images[0], pctUser);

                grpLogin.Visible = false;
                pnlUser.Visible = true;
                GetSites(this.currentUser);

                foreach(Profiles profile in this.currentUser.Profiles){
                    lstProfiles.Items.Add($"{profile.Site}\t{profile.Link}");
                }
                txtPassword.Text = "";
                txtUsername.Text = "";
                console.Out($"> Login Success!");
            } catch (UserExistsException ex)
            {
                console.Out($"> Error Loggin in with username: {username}");
                MessageBox.Show(ex.Message, "Login Issue!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } finally // Clear all user data
            {
                password = null;
                username = null;
            }
        }

        private async void setFace(string file, PictureBox picBox)
        {
            string path = PATH_TO_TEMP + file;
            await downloadImage(file);
            picBox.SizeMode = PictureBoxSizeMode.Zoom; // Setting the picture box type
            picBox.Image = new Bitmap(path);
        }

        private async Task<bool> downloadImage(string file)
        {
            string path = PATH_TO_TEMP + file;
            if (!File.Exists(path))
            {
                console.Out($"> Downloading image to: {path}");
                await BlobHandler.DownloadToTemp(path, file);
            }
            return true;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            grpLogin.Visible = true;
            pnlUser.Visible = false;
        }

        private void pnlUser_VisibleChanged(object sender, EventArgs e)
        {
            if (pnlUser.Visible)
            {
                lblWelcome.Text = String.Format("Welcome {0} {1}", currentUser.First_name, currentUser.Surname);
            }
        }

        private void btnNewProfile_Click(object sender, EventArgs e)
        {
            Form profile = new NewProfile(this);
            profile.Show();
            this.Hide();
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            btnUpload.Enabled = false;
            this.foundUsers.Clear();
            lstSimilarFaces.Items.Clear();
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Filter = "JPEG files(*.jpg)|*.jpg|PNG files(*.png)|*.png|All files (*.*)|*.*";
            tabControl1.TabPages.Clear();
            if (filedialog.ShowDialog() == DialogResult.OK)
            {
                pbarProgress.Value = 0;
                pbarProgress.Show();
                string filePath = filedialog.FileName;
                console.Out($"> Image path: {filePath}");
                compareImage = new Bitmap(filePath);
                pctCompare.Image = compareImage;

                // Test a face is present
                console.Out($"> Testing face in image");
                console.Out($"...");
                bool result = await recognitionModel.ImageisFaceAsync(compareImage);
                console.Out($"> Face in image : {result}");
                if (!result)
                {
                    pbarProgress.Value = 100;
                    MessageBox.Show("No face detected");
                }

                console.Out($"> Getting parent faces");
                List<Face> parentFaces = await recognitionModel.FindParent(compareImage);

                pbarProgress.Value = 10;

                int faceIncriment = (100 - 20);
                console.Out($"> Getting matching results");
                List<List<Face>> results = await recognitionModel.FindSimilar(parentFaces, pbarProgress, faceIncriment);
                console.Out($"> Adding results");
                foreach (List<Face> parentResults in results)
                {
                    
                    List<User> foundCycle = new List<User>();
                    foreach (Face face in parentResults)
                    {
                        if (face.Similarity > 0)
                        {
                            User matching = dbs.FindUser(face.Filename);
                            if (matching != null)
                            {
                                // If the user already exists just add an image
                                if (foundCycle.Any(item => item.Username == matching.Username))
                                {
                                    foreach (User usr in foundCycle.Where(item => item.Username == matching.Username))
                                    {
                                        usr.Images.Add(matching.Images[0]);
                                    }
                                }
                                else
                                {
                                    matching.Similarity = face.Similarity;
                                    foundCycle.Add(matching);
                                    lstSimilarFaces.Items.Add($"({(int)(Math.Round(matching.Similarity,2)*100)}%){matching.First_name}\t{matching.Surname}");
                                }
                            }
                        }
                        
                    }
                    foundUsers.Add(foundCycle);
                } 
                try
                {
                    BlobCommonActions.DeleteImage(new BlobImage(parentFaces[0].Filename, parentFaces[0].Path));
                } catch (Exception c) { }
                pbarProgress.Value = 100; // worth 10
            }
            btnUpload.Enabled = true;
        }

        private async void lstSimilarFaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lstSimilarFaces.SelectedIndex;
            User selected = foundUsers.SelectMany(x=>x).ToList()[index];

            string path = PATH_TO_TEMP + selected.Images[0];
            await downloadImage(selected.Images[0]);
            Bitmap image = new Bitmap(path);

            selected.Profiles = dbs.GetUserProfiles(selected.Username);

            bool exists = false;
            for(int i =0; i < tabControl1.TabCount; i++) 
            { 
                if(tabControl1.TabPages[i].Text == $"{selected.First_name} {selected.Surname}")
                {
                    exists = true;
                }
            }
            if (!exists)
            {
                tabControl1.Controls.Add(new UserTabPage(selected.Username, $"{selected.First_name} " +
                    $"{selected.Surname}", selected, image));
                tabControl1.SelectedIndex = tabControl1.TabCount - 1;
            }
        }

        private void HomePage_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Delete the temp path
            try
            {
                Directory.Delete(PATH_TO_TEMP, true);
            }
            catch (Exception)
            {
            }
        }

        private void GetSites(User user)
        {
            List<Profiles> userProfiles = dbs.GetUserProfiles(user.Username);
            user.Profiles = userProfiles;
        }

        private void tabMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ctxClose.Show(this.tabControl1, e.Location);
            }
        }

        private void ctxClose_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == mnuClose)
            {
                int index = tabControl1.SelectedIndex;
                if (index > 0)
                {
                    tabControl1.SelectedIndex--;
                    tabControl1.TabPages.RemoveAt(index);
                } else if (tabControl1.TabPages.Count == 1)
                {
                    tabControl1.TabPages.Clear();
                } else
                {
                    tabControl1.SelectedIndex = 1;
                    tabControl1.TabPages.RemoveAt(index);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditUser frmEdit = new EditUser(PATH_TO_TEMP);

            frmEdit.Show();
            frmEdit.Focus();
            frmEdit = null;
        }

        private void pctUser_Click(object sender, EventArgs e)
        {

        }

        private void lblWelcome_Click(object sender, EventArgs e)
        {

        }
    }
}
