using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAI.Classes
{
    class ImageEncoder
    {
        public static void Encoder (Bitmap image, string path) 
        {
            System.Drawing.Imaging.Encoder imageEncoder;
            ImageCodecInfo imageEncoderInfo;
            EncoderParameter imageEncoderParameter;
            EncoderParameters imageEncoderParameters;

            // Encode the image as a 75L jpg
            imageEncoderInfo = GetEncoderInfo("image/jpeg");
            imageEncoder = System.Drawing.Imaging.Encoder.Quality;
            imageEncoderParameters = new EncoderParameters(1);
            imageEncoderParameter = new EncoderParameter(imageEncoder, 75L);
            imageEncoderParameters.Param[0] = imageEncoderParameter;

            image.Save(path, imageEncoderInfo, imageEncoderParameters);
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}
