using AForge.Video;
using AForge.Video.DirectShow;
using FaceAI.Exceptions;
using System.Drawing;
using System.Windows.Forms;

namespace FaceAI.Classes
{
    class Camera
    {
        private FilterInfoCollection filterInfoCollection;
        private VideoCaptureDevice captureDevice;

        private PictureBox primaryImage;
        private PictureBox secondaryImage;

        private Bitmap origional;
        private Bitmap resized;

        public Camera(PictureBox primaryImage, VideoCaptureDevice captureDevice = null, PictureBox secondaryImage = null)
        {
            // Transporting the selected images and capture device
            this.primaryImage = primaryImage;
            this.captureDevice = captureDevice;
            this.secondaryImage = secondaryImage;

            primaryImage.SizeMode = PictureBoxSizeMode.CenterImage; // Setting the picture box type

            if (secondaryImage != null)
            {
                secondaryImage.SizeMode = PictureBoxSizeMode.CenterImage; // Setting the picture box type
            }

            // Ensuring a capera is enabled if one was never set.
            if (captureDevice == null)
            {
                filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                this.captureDevice = new VideoCaptureDevice(filterInfoCollection[0].MonikerString);
            }
           
        }
 

        public void Run()
        {
            // Start the camera or throw an error if a camera is not detected.
            if (captureDevice != null)
            {
                captureDevice.NewFrame += VideoCaptureDevice_NewFrame;
                captureDevice.Start();

            } else
            {
                throw new MissingCameraException("No camera has been detected.");
            }
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // How to update the new frames and output them.
            this.origional = (Bitmap)eventArgs.Frame.Clone();
            this.resized = new Bitmap(origional, new Size(origional.Width / 4, origional.Height / 4));
            primaryImage.Image = resized;
        }

        public Bitmap TakePicture(bool secondary = false, bool stop = false)
        {
            // Ensure the camera is running (need to get a new frame)
            if (captureDevice.IsRunning)
            {
                // Take copies of this
                if(this.origional == null)
                {
                    // Throw and error if a camera was not detected
                    throw new CameraNotActiveException("No camera is currently running so a picture cannot be captures.");
                }
                    Bitmap origionalCopy = (Bitmap)this.origional.Clone();
                    Bitmap resizedCopy = (Bitmap)this.resized.Clone();

                    // If we have told it to stop (single output) then stop the camera running
                    if (stop)
                    {
                        this.captureDevice.SignalToStop();
                    }

                    // The output medium
                    if (secondary)
                    {
                        this.secondaryImage.Image = resizedCopy;
                    } else
                    {
                        this.primaryImage.Image = resizedCopy;
                    }

                    // Return the origional bitmap
                    return origionalCopy;

            } else
            {
                // Throw and error if a camera was not detected
                throw new CameraNotActiveException("No camera is currently running so a picture cannot be captures.");

            }
        }
        
        public void CloseCamera()
        {
            // Stop the camera from running. This SHOULD be closed when a form running the camera is closed
            if (captureDevice.IsRunning)
            {
                captureDevice.SignalToStop();
            }
        }
    }
}
