using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceAI.Classes
{
    class Face
    {
        string filename;
        string path;
        double similarity;
        Guid? imageGuid;
        FaceRectangle rectangle;
        DetectedFace detectedFace;

        public Face(string filename, string filepath, double similarity, Guid? guid, FaceRectangle rectangle, DetectedFace detectedFace = null)
        {
            this.Filename = filename;
            this.Similarity = similarity;
            this.ImageGuid = guid;
            this.Rectangle = rectangle;
            this.path = filepath;
            this.DetectedFace = detectedFace;
        }

        public string Filename { get => filename; set => filename = value; }
        public string Path { get => path; set => path = value; }
        public double Similarity { get => similarity; set => similarity = value; }
        public Guid? ImageGuid { get => imageGuid; set => imageGuid = value; }
        public FaceRectangle Rectangle { get => rectangle; set => rectangle = value; }
        public DetectedFace DetectedFace { get => detectedFace; set => detectedFace = value; }
    }
}
