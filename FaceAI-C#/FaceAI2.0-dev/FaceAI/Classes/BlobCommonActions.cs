using FaceAI.Azure.Database;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAI.Classes
{
    class BlobCommonActions
    {
        public static async Task<BlobImage> SaveImageAsync(string tempPath, Bitmap image)
        {
            // Generate a filename as a hash of the current datetime and some random number
            DateTime foo = DateTime.Now;
            Random rnd = new Random();
            long val = rnd.Next(1111111, 779999999);
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds() + val;
            string file_name = unixTime.ToString() + ".jpg";
            // Path to temporary save location
            string path = tempPath + file_name;

            // Save the bitmat as a jpg to the temporary llocation
            ImageEncoder.Encoder(image, path);


            // Upload this to the blob
            BlobImage blobImage = await BlobHandler.UploadToStorage(path, file_name);

            // Return the filename to access
            return blobImage;
        }

        public static void DeleteImage(BlobImage image)
        {
            BlobHandler.DeleteItem(image.Filename);
        }

        public static async Task<List<string>> GetFilesAsync() {
            return await BlobHandler.GetFilesAsync();
        }
    }
}
