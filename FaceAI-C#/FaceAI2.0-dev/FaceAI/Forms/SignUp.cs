using FaceAI.Azure.Database;
using FaceAI.Classes;
using FaceAI.Exceptions;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceAI.Forms
{
    public partial class SignUp : Form
    {
        private Form parent;
        private Database database;
        private Camera camera;
        private Bitmap image;

        private readonly string PATH_TO_TEMP;

        public SignUp(Form parent, string PATH_TO_TEMP)
        {
            InitializeComponent();
            this.parent = parent; // The parent form to be able to reopen without spawning a new form.
            database = new Database();

            this.PATH_TO_TEMP = PATH_TO_TEMP;

            // Initilize the camera and start it
            camera = new Camera(pctUser);
            camera.Run();
        }

        private void SignUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close the thread running the camera.
            camera.CloseCamera();
            parent.Show();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtRePassword.Text) // Check that both the passwords match if they don't throw Display message to user
            {
                MessageBox.Show("The passwords do not match, please re-enter your passwords.", "Passwords do not match!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else if (this.image == null) { // Ensure that a user image has been taken for connection query
                MessageBox.Show("No image has been taken. Please take an image", "No user image!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else
            {
                // Generate a new user object
                User newUser = new User(txtUsername.Text.Trim(), txtPassword.Text.Trim(), txtFirstName.Text.Trim(), txtSurname.Text.Trim());

                // Try to save the user to the database
                try
                {
                    BlobImage blobImage = await BlobCommonActions.SaveImageAsync(PATH_TO_TEMP, (Bitmap)image.Clone());
                    database.NewUser(newUser, blobImage.Filename);
                    MessageBox.Show("User has been added!", "Success!", MessageBoxButtons.OK);
                    // Resolve the signup by returning the user to the home screen.
                    parent.Show();
                    this.Close();
                } catch (Exception ex) when (ex is DatabaseInsertException || ex is UserExistsException) // Catch errors 
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }

        private void btnTakePicture_Click(object sender, EventArgs e)
        {
            try
            {
                // Take a picture of the User 
                image = camera.TakePicture(stop: true);

                // Hide buttons and allow user to retake image
                btnTakePicture.Visible = false;
                btnRetake.Visible = true;
            } catch (CameraNotActiveException ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnRetake_Click(object sender, EventArgs e)
        {
            // Restart the camera
            camera.Run();
            btnTakePicture.Visible = true;
            btnRetake.Visible = false;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Filter = "JPEG files(*.jpg)| *.jpg |PNG files(*.png)| *.png|All files (*.*)|*.*";
            if (filedialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = filedialog.FileName;
                pctUser.SizeMode = PictureBoxSizeMode.Zoom;

                image = new Bitmap(filePath);
                pctUser.Image = image;
            }
        }




        // Image encoder function

    }
}
